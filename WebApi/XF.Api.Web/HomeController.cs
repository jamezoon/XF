using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

using XF.Api.Core;
using XF.Api.Lib;

using XFramework.Util;
using XFramework.Safe;
using XFramework.Mvc;
using XFramework.Entity;

namespace Mola.Api.Web
{
    public class HomeController : BaseController
    {
        [AuthFilter]
        public JsonResult Invoke(string ServiceKey, string ServiceMethed, [ModelBinder(typeof(ArgsModelBinder))]string ServiceArgs)
        {
            HttpAdapter inv = new HttpAdapter();

            object result = inv.Invoker(ServiceKey, ServiceMethed, ServiceArgs);

            return Json(new SingleResult<object>() { Data = result, Success = true, Message = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return Content("welcome to web api");
        }

        public ActionResult ClearCache()
        {
            if (ApiService.Instance.ClearCache())
            {
                return Content("本机缓存清除成功");
            }
            else
            {
                return Content("本机缓存清除失败");
            }
        }
    }
}
