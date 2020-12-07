using Model.Dao;
using Model.EF;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace OnlineShop.Areas.Admin.Controllers
{
    public class FooterController : BaseController
    {
        // GET: Admin/Footer
        [HasCredential(RoleID = "VIEW_FOOTER")]
        public ActionResult Index(string searchString = "", int page = 1, int pageSize = 10)
        {
            var dao = new FooterDao();
            var model = dao.ListAllPaging(searchString, page, pageSize);
            ViewBag.SearchString = searchString;
            return View(model);
        }

        //
        [HasCredential(RoleID = "ADD_PRODUCT")]
        public ActionResult Insert(Product model)
        {
            if (ModelState.IsValid)
            {
                var dao = new ProductDao();
                long id = dao.Insert(model);
                if (id > 0)
                {
                    return RedirectToAction("Create", "Product");
                }
                else
                {
                    ModelState.AddModelError("", "Thêm sản phẩm không thành công");
                    return View(model);
                }
            }
            return View("Create");
        }

        //done
        [HttpGet]
        [HasCredential(RoleID = "ADD_FOOTER")]
        public ActionResult Create()
        {
            return View("Create");
        }

        //done
        [ValidateInput(false)]
        [HttpPost]
        [HasCredential(RoleID = "ADD_FOOTER")]
        public ActionResult Create(Footer model)
        {
            if (ModelState.IsValid)
            {
                new FooterDao().Create(model);
                return RedirectToAction("Index");
            }
            return View();
        }
        //done
        [HttpGet]
        [HasCredential(RoleID = "EDIT_FOOTER")]
        public ActionResult Edit(long id)
        {
            var dao = new FooterDao();
            var footer = dao.GetByID(id);
            return View(footer);
        }

        //done
        [ValidateInput(false)]
        [HttpPost]
        [HasCredential(RoleID = "EDIT_FOOTER")]
        public ActionResult Edit(Footer model)
        {
            if (ModelState.IsValid)
            {
                new FooterDao().Update(model);
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpDelete]
        [HasCredential(RoleID = "DELETE_FOOTER")]
        public ActionResult Delete(long id)
        {
            new FooterDao().Delete(id);
            SetAlert("Xóa footer thành công", "success");
            return RedirectToAction("Index", "Footer");
        }
        //done
        [HttpPost]
        [HasCredential(RoleID = "EDIT_FOOTER")]
        public JsonResult ChangeStatus(long id)
        {
            var result = new FooterDao().ChangeStatus(id);
            return Json(new
            {
                status = result
            });
        }

        [HasCredential(RoleID = "VIEW_PRODUCT")]
        public ActionResult Detail(long id)
        {
            var model = new FooterDao().GetByID(id);
            return View(model);
        }

    }
}