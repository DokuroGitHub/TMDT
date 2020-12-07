using BotDetect.Web.Mvc;
using Model.Dao;
using Model.EF;
using OnlineShop.Common;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace OnlineShop.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        // GET: Admin/User
        [HasCredential(RoleID = "VIEW_USER")]
        public ActionResult Index(string searchString, int page = 1)
        {
            var dao = new UserDao();
            var model = dao.ListAllPaging(searchString, page);
            ViewBag.SearchString = searchString;
            return View(model);
        }

        public ActionResult Insert(User user)
        {
            if (ModelState.IsValid)
            {
                var dao = new UserDao();
                long id = dao.Insert(user);
                if (id > 0)
                {
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    ModelState.AddModelError("", "Thêm user không thành công");
                    return View(user);
                }
            }
            return View("Index");
        }

        [HttpGet]
        [HasCredential(RoleID = "ADD_USER")]
        public ActionResult Create()
        {
            return View();
        }
        //da co language
        [HttpPost]
        [HasCredential(RoleID = "ADD_USER")]
        public ActionResult Create(User model)
        {
            if (ModelState.IsValid)
            {
                var dao = new UserDao();
                //xử lý UserName
                if (string.IsNullOrEmpty(model.UserName))
                {
                    ModelState.AddModelError("", "Tên đăng nhập không được để trống.");
                    return View(model);
                }
                //đổi tên + tên trùng
                else if (dao.CheckUserName(model.UserName) == true)
                {
                    ModelState.AddModelError("", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }
                //đổi email + email trùng
                else if (dao.CheckEmail(model.Email))
                {
                    ModelState.AddModelError("", "Email đã tồn tại.");
                    return View(model);
                }
                else if (model.Password==null)
                {
                    ModelState.AddModelError("", "Mật khẩu không được để trống.");
                    return View(model);
                }
                //lấy thông tin từ session trong OnlineShop
                var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];
                var culture = Session[Common.CommonConstants.CurrentCulture];
                model.Language = culture.ToString();
                var ddMMyyyy = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss tt");
                var MMddyyyy = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss tt");
                //cần thêm trường language
                model.CreatedBy = session.UserName;
                if (model.Language == "vi")
                {
                    model.CreatedDate = DateTime.Parse(ddMMyyyy);
                }
                else if (model.Language == "en")
                {
                    model.CreatedDate = DateTime.Parse(MMddyyyy);
                }
                else
                    model.CreatedDate = null;
                if (!string.IsNullOrEmpty(model.Password))
                {
                    var encryptedMD5Pas = Encryptor.MD5Hash(model.Password);
                    model.Password = encryptedMD5Pas;
                }
                //nhét vào update
                var result = dao.Insert(model);
                if (result > 0)
                {
                    ViewBag.Success = "Edit thành công.";
                    //2 dòng clear model
                    model = new User();
                    ModelState.Clear();
                    SetAlert("Thêm user thành công", "success");
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    ModelState.AddModelError("", "Thêm user không thành công.");
                    return View(model);
                }
            }
            return View("Index");
        }

        [HasCredential(RoleID = "EDIT_USER")]
        public ActionResult Edit(long id)
        {
            var user = new UserDao().GetByID(id);
            SetViewBag(user.GroupID);
            return View(user);
        }
        //da co language
        [HttpPost]
        [HasCredential(RoleID = "EDIT_USER")]
        public ActionResult Edit(User model)
        {
            if (ModelState.IsValid)
            {
                var dao = new UserDao();
                //xử lý UserName
                if (string.IsNullOrEmpty(model.UserName))
                {
                    ModelState.AddModelError("", "Tên đăng nhập không được để trống.");
                    return View(model);
                }
                //đổi tên + tên trùng
                else if (dao.GetByID(model.ID).UserName != model.UserName && dao.CheckUserName(model.UserName) == true)
                {
                    ModelState.AddModelError("", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }
                //đổi email + email trùng
                else if (dao.GetByID(model.ID).Email != model.Email && dao.CheckEmail(model.Email))
                {
                    ModelState.AddModelError("", "Email đã tồn tại.");
                    return View(model);
                }
                //lấy thông tin từ session trong OnlineShop
                var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];              
                var culture = Session[Common.CommonConstants.CurrentCulture];
                var ddMMyyyy = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss tt");
                var MMddyyyy = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss tt");
                model.Language = culture.ToString();
                model.ModifiedBy = session.UserName;
                if (model.Language == "vi")
                {
                    model.ModifiedDate = DateTime.Parse(ddMMyyyy);
                }
                else if (model.Language == "en")
                {
                    model.ModifiedDate = DateTime.Parse(MMddyyyy);
                }
                else
                    model.ModifiedDate = null;
                if (!string.IsNullOrEmpty(model.Password))
                {
                    var encryptedMD5Pas = Encryptor.MD5Hash(model.Password);
                    model.Password = encryptedMD5Pas;
                }
                //nhét vào update
                var result = dao.Update(model);
                if (result > 0)
                {
                    ViewBag.Success = "Edit thành công.";
                    //2 dòng clear model
                    model = new User();
                    ModelState.Clear();
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    ModelState.AddModelError("", "Edit không thành công.");
                    return View(model);
                }
            }
            SetViewBag(model.GroupID);
            return View("Index");
        }

        [HttpDelete]
        [HasCredential(RoleID = "DELETE_USER")]
        public ActionResult Delete(int id)
        {
            new UserDao().Delete(id);

            SetAlert("Xóa user thành công", "success");
            return RedirectToAction("Index", "User");
        }

        [HttpPost]
        [HasCredential(RoleID = "EDIT_USER")]
        public JsonResult ChangeStatus(long id)
        {
            var result = new UserDao().ChangeStatus(id);
            return Json(new
            {
                status = result
            });
        }
        //done
        public JsonResult LoadProvince()
        {
            var xmlDoc = XDocument.Load(Server.MapPath(@"~/assets/client/data/Provinces_Data.xml"));

            var xElements = xmlDoc.Element("Root").Elements("Item").Where(x => x.Attribute("type").Value == "province");
            var list = new List<ProvinceModel>();
            ProvinceModel province = null;

            foreach (var item in xElements)
            {
                province = new ProvinceModel();
                province.ID = int.Parse(item.Attribute("id").Value);
                province.Name = item.Attribute("value").Value;
                list.Add(province);
            }
            return Json(new
            {
                data = list,
                status = true
            });
        }
        //done
        public JsonResult LoadDistrict(int provinceID)
        {
            var xmlDoc = XDocument.Load(Server.MapPath(@"~/assets/client/data/Provinces_Data.xml"));

            var xElements_Province = xmlDoc.Element("Root").Elements("Item").Single(x => x.Attribute("id").Value == provinceID.ToString());
            //xElements_Province khác kiểu với xmlDoc nên không lấy được xElements_District
            //var xElements_District = xElements_Province.Elements("Item").Where(x => x.Attribute("type").Value == "district");
            var list = new List<DistrictModel>();
            DistrictModel district = null;
            //Lấy Elements district của provinceID
            foreach (var item in xElements_Province.Elements("Item").Where(x => x.Attribute("type").Value == "district"))
            {
                district = new DistrictModel();
                district.ID = int.Parse(item.Attribute("id").Value);
                district.Name = item.Attribute("value").Value;
                list.Add(district);
            }
            return Json(new
            {
                data = list,
                status = true
            });
        }

        public void SetViewBag(string userGroupID = null)
        {         
            var dao = new UserGroupDao();
            if (userGroupID != null)
            {
                var selectedId = dao.ViewDetail(userGroupID).ID;
                ViewBag.GroupID = new SelectList(dao.ListAll(), "ID", "Name", selectedId);
            }
            else
            {
                var selectedId = dao.ViewDetail("MEMBER").ID;
                ViewBag.GroupID = new SelectList(dao.ListAll(), "ID", "Name", selectedId);
            }
        }
    }
}