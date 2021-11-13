using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ZaloPayEx
{

    public class BaseResult : ActionResult
    {
        public bool success { set; get; }
        public string message { set; get; }
        public object tag { set; get; }

        public override void ExecuteResult(ControllerContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class OkResult : BaseResult
    {
        public OkResult()
        {
            success = true;
        }

        public OkResult(object data)
        {
            success = true;
            tag = data;
        }
    }

    public class ErrorResult : BaseResult
    {

        public ErrorResult(string msg)
        {
            success = false;
            message = msg;
        }
    }
}
