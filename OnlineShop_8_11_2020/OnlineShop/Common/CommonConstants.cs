using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Common
{
    public static class CommonConstants
    {
        //đặt tên session
        public static string USER_SESSION = "USER_SESSION";
        public static string SESSION_CREDENTIALS = "SESSION_CREDENTIALS";
        public static string CART_SESSION = "CART_SESSION";
        public static string COUPON_SESSION = "COUPON_SESSION";

        public static string CurrentCulture { set; get; }

    }
}