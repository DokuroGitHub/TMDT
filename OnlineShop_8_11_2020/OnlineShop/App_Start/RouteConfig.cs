using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OnlineShop
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // BotDetect requests must not be routed
            routes.IgnoreRoute("{*botdetect}",
              new { botdetect = @"(.*)BotDetectCaptcha\.ashx" });

            //Rss//doing
            routes.MapRoute(
                name: "PostFeed",
                url: "Feed/{type}",
                defaults: new { controller = "Blog", action = "PostFeed", type = "tin-la-chuoi", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );
            //rss//doing
            routes.MapRoute(
                name: "ReadRSS",
                url: "rss",
                defaults: new { controller = "Blog", action = "ReadRSS", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );
            //sản phẩm theo danh mục và theo trang
            routes.MapRoute(
                name: "Product by Category by page",
                url: "san-pham/{metatitle}-{categoryID}-page={pageIndex}",
                defaults: new { controller = "Product", action = "ProductWithCategoryID", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );
            //sản phẩm theo trang
            routes.MapRoute(
                name: "Product by page",
                url: "san-pham-page={pageIndex}",
                defaults: new { controller = "Product", action = "AllProduct", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );
            //Chi tiết sản phẩm trong ProductCategory.cshtml
            routes.MapRoute(
                name: "Product Detail",
                url: "chi-tiet/{metatitle}-{prodId}",
                defaults: new { controller = "Product", action = "Detail", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );
            //Tìm kiếm sản phẩm
            routes.MapRoute(
                name: "Search",
                url: "tim-kiem/page={pageIndex}",
                defaults: new { controller = "Product", action = "Search", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );
            //trong TopMenu.cshtml
            //--About
            routes.MapRoute(
                name: "About",
                url: "gioi-thieu",
                defaults: new { controller = "About", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );
            //tin tức theo danh mục và theo trang
            routes.MapRoute(
                name: "Content by Category by page",
                url: "tin-tuc/{metatitle}-{categoryID}-page={pageIndex}",
                defaults: new { controller = "Content", action = "ContentWithCategoryID", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );
            //--Content detail
            routes.MapRoute(
                name: "content detail",
                url: "tin-tuc/{metatitle}-{id}",
                defaults: new { controller = "Content", action = "Detail", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );
            //--Content by tag
            routes.MapRoute(
                name: "content by tag",
                url: "tag/{tagId}",
                defaults: new { controller = "Content", action = "Tag", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );
            //--Content
            routes.MapRoute(
                name: "News",
                url: "tin-tuc",
                defaults: new { controller = "Content", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );
            //--Product
            routes.MapRoute(
                name: "Product",
                url: "san-pham",
                defaults: new { controller = "Product", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );

            //--Contact
            routes.MapRoute(
                name: "Contact",
                url: "lien-he",
                defaults: new { controller = "Contact", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );
            //--Login
            routes.MapRoute(
                name: "Login",
                url: "dang-nhap",
                defaults: new { controller = "User", action = "Login", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );
            //--Register
            routes.MapRoute(
                name: "Register",
                url: "dang-ky",
                defaults: new { controller = "User", action = "Register", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );
            //--Cart
            routes.MapRoute(
                name: "Cart",
                url: "gio-hang",
                defaults: new { controller = "Cart", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );
            //--Payment
            routes.MapRoute(
                name: "MyShipList",
                url: "don-dat-hang-cua-toi",
                defaults: new { controller = "Cart", action = "MyShipList", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );
            //--Add to cart
            routes.MapRoute(
                name: "Add to Cart",
                url: "them-gio-hang",
                defaults: new { controller = "Cart", action = "AddItem", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );
            //--Paypal Payment
            routes.MapRoute(
                name: "Paypal Payment GET",
                url: "thanh-toan-paypal",
                defaults: new { controller = "Cart", action = "PaymentWithPaypal", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );
            //--Payment
            routes.MapRoute(
                name: "Payment",
                url: "thanh-toan",
                defaults: new { controller = "Cart", action = "Payment", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );
            //--Payment success
            routes.MapRoute(
                name: "SendOrder Success",
                url: "hoan-thanh",
                defaults: new { controller = "Cart", action = "Success", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );
            //--Payment error
            routes.MapRoute(
                name: "SendOrder error",
                url: "loi-dat-hang",
                defaults: new { controller = "Cart", action = "Error", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );
            //--Payment error
            routes.MapRoute(
                name: "Product run out",
                url: "het-san-pham",
                defaults: new { controller = "Cart", action = "ProductRunOut", id = UrlParameter.Optional },
                namespaces: new[] { "OnlineShop.Controllers" }
            );

            //Default đặt cuối cùng
            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
            //    namespaces: new[] { "OnlineShop.Controllers" }
            //);

            //dafault test
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "OnlineShop.Controllers" }
            );
        }
    }
}
