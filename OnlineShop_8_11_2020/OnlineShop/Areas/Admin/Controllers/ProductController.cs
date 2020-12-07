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
    public class ProductController : BaseController
    {
        // GET: Admin/Product//done
        [HasCredential(RoleID = "VIEW_PRODUCT")]
        public ActionResult Index(string searchString = "", int page = 1, int pageSize = 10)
        {
            var dao = new ProductDao();
            var model = dao.ListAllPaging(searchString, page, pageSize);
            ViewBag.SearchString = searchString;
            return View(model);
        }

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

        public void SetViewBag(long? selectedId = null)
        {
            var dao = new ProductCateogryDao();
            ViewBag.CategoryID = new SelectList(dao.ListAll(), "ID", "Name", selectedId);
        }

        [HttpGet]
        [HasCredential(RoleID = "ADD_PRODUCT")]
        public ActionResult Create()
        {
            SetViewBag();
            return View("Create");
        }

        //phải có validateinput false để dùng ckfinder
        [ValidateInput(false)]
        [HttpPost]
        [HasCredential(RoleID = "ADD_PRODUCT")]
        public ActionResult Create(Product model)
        {
            if (ModelState.IsValid)
            {
                //lấy thông tin từ session trong OnlineShop
                var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];
                var culture = Session[Common.CommonConstants.CurrentCulture];
                model.CreatedBy = session.UserName;
                model.Language = culture.ToString();
                //create product
                new ProductDao().Create(model);
                return RedirectToAction("Index");
            }
            SetViewBag();
            return View();
        }

        [HttpGet]
        [HasCredential(RoleID = "EDIT_PRODUCT")]
        public ActionResult Edit(long id)
        {
            var dao = new ProductDao();
            var product = dao.GetByID(id);
            SetViewBag(product.CategoryID);
            return View(product);
        }

        //phải có////cần fix lỗi datetime theo language
        [ValidateInput(false)]
        [HttpPost]
        [HasCredential(RoleID = "EDIT_PRODUCT")]
        public ActionResult Edit(Product model)
        {
            if (ModelState.IsValid)
            {
                //lấy thông tin từ session trong OnlineShop
                var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];
                model.ModifiedBy = session.UserName;
                //update product
                new ProductDao().Update(model);
                return RedirectToAction("Index");
            }
            SetViewBag(model.CategoryID);
            return View();
        }

        [HttpDelete]
        [HasCredential(RoleID = "DELETE_PRODUCT")]
        public ActionResult Delete(long id)
        {
            new ProductDao().Delete(id);
            SetAlert("Xóa sản phẩm thành công", "success");
            return RedirectToAction("Index", "Product");
        }

        [HttpPost]
        [HasCredential(RoleID = "EDIT_PRODUCT")]
        public JsonResult ChangeStatus(long id)
        {
            var result = new ProductDao().ChangeStatus(id);
            return Json(new
            {
                status = result
            });
        }

        [HasCredential(RoleID = "VIEW_PRODUCT")]
        public ActionResult Detail(long id)
        {
            var model = new ProductDao().GetByID(id);
            return View(model);
        }

        public JsonResult SaveImages(long id,string images)
        {
            //deserialize chuỗi string images
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var listImages = serializer.Deserialize<List<string>>(images);

            XElement xElement = new XElement("Images");
            foreach(var item in listImages)
            {
                //lấy substring của image src
                var subStringItem = item.Substring(23);
                xElement.Add(new XElement("Image", subStringItem));
            }
            ProductDao dao = new ProductDao();
            try
            {
                dao.UpdateImages(id, xElement.ToString());
                return Json(new
                {
                    status = true
                });
            }
            catch
            {
                return Json(new
                {
                    status = false
                });
            }
        }

        public JsonResult LoadImages(long id)
        {
            var product = new ProductDao().GetByID(id);
            var images = product.MoreImages;
            XElement xImages;
            List<string> ListImagesReturn = new List<string>();
            if (images != null)
            {
                xImages = XElement.Parse(images);
                foreach (XElement element in xImages.Elements())
                {
                    ListImagesReturn.Add(element.Value);
                }
            }
            return Json(new
            {
                data = ListImagesReturn
            }, JsonRequestBehavior.AllowGet); //cho phép gọi GET
        }
    }
}