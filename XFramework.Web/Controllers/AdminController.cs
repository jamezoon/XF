using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using XFramework.Safe;
using XFramework.BLL;
using XFramework.Entity;

namespace XFramework.Web.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Category()
        {
            int categoryID = QueryString.Int32SafeQ("category");

            IList<CategoryEntity> categoryList = BaseBLL<CategoryEntity>.Instance.GetList(x => x.ParentID == categoryID, x => x.OrderID);

            if (categoryID == 0)
            {
                return View(categoryList);
            }
            else
            {
                return Json(categoryList.Select(c => new
                {
                    c.CategoryID,
                    c.CategoryName,
                    CreateTime = c.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    ModifyTime = c.ModifyTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    c.OrderID,
                    c.CategoryDesc
                }), JsonRequestBehavior.DenyGet);
            }
        }
    }
}
