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

namespace OnlineShop.Controllers
{
    public class UserController : BaseController
    {
        //Điều hướng từ trang facebook vào hàm lấy thông tin fb gửi về
        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        // GET: User
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [CaptchaValidationActionFilter("CaptchaCode", "registerCaptcha", "Mã captcha không đúng!")]
        public ActionResult Register(Models.RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var dao = new UserDao();
                if (dao.CheckUserName(model.UserName))
                {
                    ModelState.AddModelError("", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }
                else if (dao.CheckEmail(model.Email))
                {
                    ModelState.AddModelError("", "Email đã tồn tại.");
                    return View(model);
                }
                else if (model.Password == null)
                {
                    ModelState.AddModelError("", "Mật khẩu không được để trống.");
                    return View(model);
                }
                else if(model.ConfirmPassword != model.Password)
                {
                    ModelState.AddModelError("", "Xác nhận mật khẩu không đúng.");
                    return View(model);
                }
                else
                {
                    var user = new User();
                    user.UserName = model.UserName;
                    user.Password = OnlineShop.Common.Encryptor.MD5Hash(model.Password);
                    user.GroupID = "MEMBER";
                    user.Name = model.Name;
                    user.Address = model.Address;
                    user.Email = model.Email;
                    user.Phone = model.Phone;
                    user.CreatedDate = DateTime.Now;
                    user.CreatedBy = model.UserName;
                    user.Status = true;
                    user.ProvinceID = model.ProvinceID != null ? model.ProvinceID : null;
                    user.DistrictID = model.DistrictID != null ? model.DistrictID : null;

                    var result = dao.Insert(user);
                    if (result > 0)
                    {
                        ViewBag.Success = "Đăng ký thành công.";
                        //2 dòng clear model
                        model = new Models.RegisterModel();
                        ModelState.Clear();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Đăng ký không thành công.");
                        return View(model);
                    }
                }
            }
            return View(model);
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
                //không là login với quyền quản trị
                var result = dao.Login(model.UserName, Encryptor.MD5Hash(model.Password), 3); //Admin(1) Shipper(2) User(3)
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
                    Session.Add(OnlineShop.Common.CommonConstants.USER_SESSION, userSession);
                    Session.Add(OnlineShop.Common.CommonConstants.SESSION_CREDENTIALS, listCredentials);

                    //Session["isAuthorized"] = true;
                    return RedirectToAction("Index", "Home");
                }
                else if (result == 0)
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
            //model ko valid
            return View("Index");
        }

        //Khi nhấn nút login vào facebook
        public ActionResult LoginFacebook(LoginModel model)
        {
            var fb = new Facebook.FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = System.Configuration.ConfigurationManager.AppSettings["FbAppId"],
                client_secret = System.Configuration.ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email",
            });
            return Redirect(loginUrl.AbsoluteUri);
        }

        //Lấy thông tin fb gửi về //chưa fix thông tin credential
        public ActionResult FacebookCallback(string code)
        {
            var fb = new Facebook.FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = System.Configuration.ConfigurationManager.AppSettings["FbAppId"],
                client_secret = System.Configuration.ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code,
            });
            var accessToken = result.access_token;
            if (!string.IsNullOrEmpty(accessToken))
            {
                fb.AccessToken = accessToken;
                //Lấy thông tin được gửi về
                dynamic me = fb.Get("me?fields=first_name,middle_name,last_name,id,email");
                string email = me.email;
                string userName = me.email;
                string firstname = me.first_name;
                string middlename = me.middle_name;
                string lastname = me.last_name;
                //Tạo mới 1 User và gán thông tin vào user
                var user = new User();
                user.Email = email;
                user.UserName = userName;
                user.Status = true;
                user.Name = firstname + " " + middlename + " " + lastname;
                user.CreatedDate = DateTime.Now;
                //Ghi vào database
                var id = new UserDao().InsertForFacebook(user);
                var userSession = new UserLogin();
                userSession.UserName = user.UserName;
                userSession.UserID = user.ID;

                Session.Add(OnlineShop.Common.CommonConstants.USER_SESSION, userSession);
            }
            return Redirect("/");
        }
        //done
        public ActionResult Logout(LoginModel model)
        {
            Session[OnlineShop.Common.CommonConstants.USER_SESSION] = null;
            Session[OnlineShop.Common.CommonConstants.SESSION_CREDENTIALS] = null;
            //Session[CommonConstants.isAuthorized] = false;
            //return RedirectToAction("Index", "Home");
            return Redirect("/");
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
    }
}