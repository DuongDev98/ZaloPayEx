using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZaloPayEx.Controllers
{
    public partial class ZalopayController : Controller
    {
        string appid;
        string keyApp;
        string merchantInfo;

        string createOrderUrl = "https://sandbox.zalopay.com.vn/v001/tpe/createorder";
        string queryOrderUrl = "https://sandbox.zalopay.com.vn/v001/tpe/getstatusbyapptransid";

        public ZalopayController()
        {
            appid = "";
            keyApp = "";
            merchantInfo = "";
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> GetOrderInfoAsync(string TDONHANGID)
        {
            string apptransid = GetAppTransId(DateTime.Now, "ID");

            var param = new Dictionary<string, string>();
            param.Add("appid", appid);
            param.Add("apptransid", apptransid);
            var data = appid + "|" + apptransid + "|" + keyApp;
            param.Add("mac", ZaloPayHmacHelper.Compute(ZaloPayHMAC.HMACSHA256, keyApp, data));
            Dictionary<string, object> result = await ZaloPayHttpHelper.PostFormAsync(queryOrderUrl, param);

            int retCode = Convert.ToInt32(result["returncode"]);
            bool isProcessing = Convert.ToBoolean(result["isprocessing"]);
            string retMessage = Convert.ToString(result["returnmessage"]);

            if (isProcessing || retCode == -49 || retCode == -117 || retCode == 1)
            {
                return new OkResult(result);
            }
            else
                return new ErrorResult(retMessage.ToUpper());
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> ZaloPayAsync(string TDONHANGID)
        {
            try
            {
                List<ZaloItemOrder> items = new List<ZaloItemOrder>();
                StoreInfo storeInfo = new StoreInfo() { merchantinfo = merchantInfo, redirecturl = "https://docs.zalopay.vn/result" };

                decimal tongCong = 0;
                string id = "";
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("appid", appid);
                dic.Add("appuser", merchantInfo);
                dic.Add("apptime", ZaloPayUtils.GetTimeStamp().ToString());
                dic.Add("amount", Convert.ToString((int)tongCong));
                dic.Add("apptransid", GetAppTransId(DateTime.Now, id));
                dic.Add("embeddata", JsonConvert.SerializeObject(storeInfo));
                dic.Add("item", JsonConvert.SerializeObject(items));
                dic.Add("bankcode", "zalopayapp");
                dic.Add("description", "THANH TOÁN ĐƠN HÀNG");
                string data = appid + "|" + dic["apptransid"] + "|" + dic["appuser"] + "|" + dic["amount"] + "|" + dic["apptime"] + "|" + dic["embeddata"] + "|" + dic["item"];
                dic.Add("mac", ZaloPayHmacHelper.Compute(ZaloPayHMAC.HMACSHA256, keyApp, data));

                Dictionary<string, object> ret = await ZaloPayHttpHelper.PostFormAsync(createOrderUrl, dic);
                int retCode = Convert.ToInt32(ret["returncode"]);
                if (retCode == -2)
                {
                    return new ErrorResult("LỖI: SAI THÔNG TIN TÀI KHOẢN ZALO");
                }
                else
                {
                    string orderUrl = ret.ContainsKey("orderurl") ? Convert.ToString(ret["orderurl"]) : string.Empty;
                    if (orderUrl.Length == 0)
                    {
                        return new ErrorResult("LỖI: KHÔNG TẠO ĐƯỢC HÓA ĐƠN ZALO");
                    }
                    else
                        return new OkResult(ret);
                }
            }
            catch (Exception e)
            {
                return new ErrorResult("LỖI KẾT NỐI ZALO: " + e.Message);
            }
        }

        // mã giao dich có định dạng yyMMdd_xxxx
        private string GetAppTransId(DateTime ngay, string ID)
        {
            return ngay.ToString("yyMMdd") + "-" + ID.Replace("-", "");
        }
    }

    class StoreInfo
    {
        public string merchantinfo { set; get; }
        public string redirecturl { set; get; }
    }

    class ZaloItemOrder
    {
        public string itemid { set; get; }
        public string itemname { set; get; }
        public decimal itemprice { set; get; }
        public decimal itemquantity { set; get; }
    }
}