using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using XFramework.BLL;
using XFramework.Entity;
using XFramework.Safe;


namespace XFramework.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //test

            IList<CategoryEntity> categoryList = BaseBLL<CategoryEntity>.Instance.GetList(x => true);

            int categoryID = QueryString.Int32SafeQ("category");

            if (categoryID > 0)
                ViewBag.CategoryEntity = BLL.CategoryBLL.Get(categoryID);

            return View(categoryList);
        }

        [ValidateInput(false)]
        public ActionResult Article()
        {
            if (Request.HttpMethod == "POST")
            {
                ArticleEntity entity = new ArticleEntity()
                {
                    ArticleTitle = FormString.SafeQ("txtTitle"),
                    ArticleDesc = FormString.SafeQ("txtDesc"),
                    CategoryID = FormString.Int32SafeQ("child"),
                    CreateTime = DateTime.Now,
                    IsRedirect = FormString.Int32SafeQ("hIsRedirect") == 1,
                    RedirectUrl = FormString.SafeQ("txtRedirectUrl"),
                    OrderID = FormString.Int32SafeQ("txtOrderID")
                };

                if (BaseBLL<ArticleEntity>.Instance.Add(entity))
                    return Content("添加成功！");
                else
                    return Content("添加失败！");
            }
            else
            {
                IList<CategoryEntity> categoryList = BaseBLL<CategoryEntity>.Instance.GetList(x => x.ParentID == 0);

                return View(categoryList);
            }
        }

        public ActionResult Category()
        {
            if (Request.HttpMethod == "POST")
            {
                CategoryEntity entity = new CategoryEntity()
                {
                    CategoryName = FormString.SafeQ("txtCategoryName"),
                    CategoryDesc = FormString.SafeQ("txtCategoryDesc"),
                    ParentID = FormString.Int32SafeQ("parent"),
                    CreateTime = DateTime.Now,
                    OrderID = FormString.Int32SafeQ("txtOrderID")
                };

                if (BaseBLL<CategoryEntity>.Instance.Add(entity))
                    return Content("添加成功！");
                else
                    return Content("添加失败！");
            }
            else
            {
                int categoryID = QueryString.Int32SafeQ("category");

                IList<CategoryEntity> categoryList = BaseBLL<CategoryEntity>.Instance.GetList(x => x.ParentID == categoryID);

                return View(categoryList);
            }
        }

        public ActionResult GetCategory()
        {
            int categoryID = FormString.Int32SafeQ("category");

            IList<CategoryEntity> categoryList = BaseBLL<CategoryEntity>.Instance.GetList(x => x.ParentID == categoryID);

            return Json(categoryList.OrderBy(x => x.OrderID).Select(x => new
            {
                x.CategoryID,
                x.CategoryName,
                x.ParentID,
                CreateTime = x.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
            }), JsonRequestBehavior.AllowGet);
        }
    }
}
