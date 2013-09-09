using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace XFramework.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Log()
        {
            //XFramework.Log.LogUtil.Log("abc", "message");

            string str = "string123";

            XFramework.Caching.Memcached.Set("abc", str, str);


            object obj = XFramework.Caching.Memcached.Get("abc", str);

            return Content("ok");
        }

        public ActionResult DB()
        {
           

            return Content("ok");
        }

    }
}
