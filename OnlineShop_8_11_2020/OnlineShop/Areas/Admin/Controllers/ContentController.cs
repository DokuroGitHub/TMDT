using Model.Dao;
using Model.EF;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Areas.Admin.Controllers
{
    public class ContentController : BaseController
    {
        // GET: Admin/Content
        [HasCredential(RoleID = "VIEW_CONTENT")]
        public ActionResult Index(string searchString="", int page = 1, int pageSize=10)
        {
            var dao = new ContentDao();
            var model = dao.ListAllPaging(searchString, page, pageSize);
            ViewBag.SearchString = searchString;
            return View(model);
        }

        [HasCredential(RoleID = "ADD_CONTENT")]
        public ActionResult Insert(Content model)
        {
            if (ModelState.IsValid)
            {
                var dao = new ContentDao();
                long id = dao.Insert(model);
                if (id > 0)
                {
                    return RedirectToAction("Create", "Content");
                }
                else
                {
                    ModelState.AddModelError("", "Thêm tin tức không thành công");
                    return View(model);
                }
            }
            return View("Create");
        }

        public void SetViewBag(long? selectedId = null)
        {
            var dao = new CategoryDao();
            ViewBag.CategoryID = new SelectList(dao.ListAll(), "ID", "Name", selectedId);
        }

        [HttpGet]
        [HasCredential(RoleID = "ADD_CONTENT")]
        public ActionResult Create()
        {
            SetViewBag();
            return View("Create");
        }

        //phải có validateinput false để dùng ckfinder
        [ValidateInput(false)]
        [HttpPost]
        [HasCredential(RoleID = "ADD_CONTENT")]
        public ActionResult Create(Content model)
        {
            if (ModelState.IsValid)
            {
                //lấy thông tin từ session trong OnlineShop
                var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];
                model.CreatedBy = session.UserName;
                var culture = Session[Common.CommonConstants.CurrentCulture];
                model.Language = culture.ToString();
                //create content
                new ContentDao().Create(model);
                return RedirectToAction("Index");
            }
            SetViewBag();
            return View();
        }

        [HttpGet]
        [HasCredential(RoleID = "EDIT_CONTENT")]
        public ActionResult Edit(long id)
        {
            var dao =new ContentDao();
            var content = dao.GetByID(id);
            SetViewBag(content.CategoryID);
            return View(content);
        }

        //phải có
        [ValidateInput(false)]
        [HttpPost]
        [HasCredential(RoleID = "EDIT_CONTENT")]
        public ActionResult Edit(Content model)
        {
            if (ModelState.IsValid)
            {
                //lấy thông tin từ session trong OnlineShop
                var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];
                model.ModifiedBy = session.UserName;
                var culture = Session[Common.CommonConstants.CurrentCulture];
                model.Language = culture.ToString();
                //update content
                new ContentDao().Update(model);
                return RedirectToAction("Index");
            }
            SetViewBag(model.CategoryID);
            return View();
        }

        [HttpDelete]
        [HasCredential(RoleID = "DELETE_CONTENT")]
        public ActionResult Delete(long id)
        {
            new ContentDao().Delete(id);
            SetAlert("Xóa tin tức thành công", "success");
            return RedirectToAction("Index", "Content");
        }

        [HttpPost]
        [HasCredential(RoleID = "EDIT_CONTENT")]
        public JsonResult ChangeStatus(long id)
        {
            var result = new ContentDao().ChangeStatus(id);
            return Json(new
            {
                status = result
            });
        }

        [HasCredential(RoleID = "VIEW_CONTENT")]
        public ActionResult Detail(long id)
        {
            var model = new ContentDao().GetByID(id);
            return View(model);
        }

    }
}