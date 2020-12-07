using Model.Dao;
using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Areas.Admin.Controllers
{
    public class CategoryController : BaseController
    {
        //Chưa làm view

        // GET: Admin/Category
        [HasCredential(RoleID = "VIEW_CATEGORY")]
        public ActionResult Index()
        {
            return View();
        }

        [HasCredential(RoleID = "ADD_CATEGORY")]
        public ActionResult Create(Category model)
        {
            if (ModelState.IsValid)
            {
                var id = new CategoryDao().Insert(model);
                if (id > 0)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("",StaticResources.Resources.InsertCategoryFailed);
                    return View(model);
                }
            }
            return View(model);
        }
    }
}