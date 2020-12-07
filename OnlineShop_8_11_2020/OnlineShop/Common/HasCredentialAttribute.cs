using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Common;

namespace OnlineShop
{
    //Kiểm tra quyền trước khi dùng hàm
    public class HasCredentialAttribute : AuthorizeAttribute
    {
        //RoleID đặt trên mỗi hàm trong controller
        public string RoleID { set; get; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //đã có 2 Session: (UserLogin())USER_SESSION, (List<string>)SESSION_CREDENTIALS
            var newUserLogin = (UserLogin)HttpContext.Current.Session[Common.CommonConstants.USER_SESSION];
            var listRoleID = (List<string>)HttpContext.Current.Session[Common.CommonConstants.SESSION_CREDENTIALS];

            //chưa log in
            if (newUserLogin == null)
            {
                return false;
            }
            //đã log in
            else
            {
                //ListRoleID rỗng
                if (listRoleID == null)
                {
                    return false;
                }
                else
                {
                    //user là admin hoặc listRoleID của GroupID có RoleID của hàm
                    //listRoleID.Contains(ConfigurationManager.AppSettings["ADMINRoleID"].ToString())
                    if (newUserLogin.GroupID == CommonConstants.ADMIN_GROUP || listRoleID.Contains(this.RoleID))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        //khi AuthorizeCore trả về false//trang hiển thị lỗi 401(access denied)
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var data = new ViewDataDictionary
            {
                new KeyValuePair<string, object>("RoleID", RoleID)
            };

            filterContext.Result = new ViewResult
            {
                ViewName = "/Areas/Admin/Views/Shared/401.cshtml",
                ViewData = new ViewDataDictionary(data)
            };
        }

    }
}