using Model.Dao;
using OnlineShop.Areas.Admin.Model;
using OnlineShop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        // GET: Admin/Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var dao = new UserDao();
                var result = dao.Login(model.UserName, Encryptor.MD5Hash(model.Password), 1); //Admin(1) Shipper(2) User(3)
                if (result == 1)
                {
                    var user = dao.GetByUserName(model.UserName);
                    var userSession = new UserLogin();
                    userSession.UserName = user.UserName;
                    userSession.UserID = user.ID;
                    userSession.GroupID = user.GroupID;
                    //UserName lấy được GroupID rồi Join 3 table: Credential + UserGroup + Role lấy listRoleID //cần fix lấy list gồm 2 cột RoleID+RoleName
                    var listCredentials = dao.GetListCredential(model.UserName);
                    
                    //tạo 2 Session: (UserLogin())USER_SESSION, (List<string>)SESSION_CREDENTIALS
                    Session.Add(CommonConstants.USER_SESSION, userSession);
                    Session.Add(CommonConstants.SESSION_CREDENTIALS, listCredentials);

                    return RedirectToAction("Index", "Home");
                }
                else if(result == 0)
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại");
                    return View(model);
                }
                else if (result == -1)
                {
                    ModelState.AddModelError("", "Tài khoản đang bị khóa");
                    return View(model);
                }
                else if (result == -2)
                {
                    ModelState.AddModelError("", "Mật khẩu không đúng");
                    return View(model);
                }
                else if (result == -3)
                {
                    ModelState.AddModelError("", "Tài khoản không có quyền đăng nhập quản trị");
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError("", "Lỗi này chưa được khám phá");
                    return View(model);
                }
            }
            return View("Index");
        }

        //done
        public ActionResult Logout(LoginModel model)
        {
            Session[CommonConstants.USER_SESSION] = null;
            Session[CommonConstants.SESSION_CREDENTIALS] = null;
            return RedirectToAction("Index", "Login");
            //return RedirectToAction("Index", "Home");
        }
    }
}