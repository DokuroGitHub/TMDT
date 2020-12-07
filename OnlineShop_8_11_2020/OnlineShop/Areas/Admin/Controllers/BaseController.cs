using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;
using OnlineShop;

namespace OnlineShop.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        //Khởi tạo culture
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (Session[Common.CommonConstants.CurrentCulture] != null)
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(Session[Common.CommonConstants.CurrentCulture].ToString());
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Session[Common.CommonConstants.CurrentCulture].ToString());
            }
            else
            {
                Session[Common.CommonConstants.CurrentCulture] = "vi";
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("vi");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("vi");
            }
        }
        //Đổi culture
        public ActionResult ChangeCulture(string ddlCulture, string returnUrl)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(ddlCulture);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(ddlCulture);

            Session[Common.CommonConstants.CurrentCulture] = ddlCulture;
            return Redirect(returnUrl);
        }

        //cần khiến hàm redirect dc thực hiện khi session null //top1 run first //đã fix xong
        protected override void OnAuthentication(AuthenticationContext filterContext)
        {
            var session = (UserLogin)Session[Common.CommonConstants.USER_SESSION];
            if (session == null || (session.GroupID != CommonConstants.ADMIN_GROUP && session.GroupID != CommonConstants.MOD_GROUP))
            {
                filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "Login", action = "Index", Area = "Admin" }));
            }
            base.OnAuthentication(filterContext);
        }

        protected void SetAlert(string message, string type)
        {
            TempData["AlertMessage"] = message;
            if (type == "success")
            {
                TempData["AlertType"] = "alert-success";
            }
            else if (type == "warning")
            {
                TempData["AlertType"] = "alert-warning";
            }
            else if (type == "error")
            {
                TempData["AlertType"] = "alert-danger";
            }
        }
    }
}