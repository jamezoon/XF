using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using XFramework.Entity;
using NLite.Data;

namespace XFramework.Web.Controllers
{
    public class CategoryController : Controller
    {
        //
        // GET: /Category/

        public ActionResult Index()
        {

            var query = XFramework.Data.Category.GetAll((x, y) => x.CategoryID == 3);

            return Content("1");
        }
    }
}