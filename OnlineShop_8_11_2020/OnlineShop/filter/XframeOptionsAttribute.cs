using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.filter
{
    public class XframeOptionsAttribute : ActionFilterAttribute
    {
        XframeOption option;
        string allowFrom;

        public string AllowFrom
        {
            get { return allowFrom; }
            set { allowFrom = value; option = XframeOption.AllowFrom; }
        }

        public XframeOption Option
        {
            get { return option; }
            set { option = value; }
        }

        public XframeOptionsAttribute()
          : this(XframeOption.Deny)
        {

        }

        public XframeOptionsAttribute(XframeOption option)
        {
            this.Option = option;
        }

        public override void OnResultExecuting(System.Web.Mvc.ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.AddHeader("x-frame-options", FormatOption(option, allowFrom));
        }

        static string FormatOption(XframeOption option, string allowFrom)
        {
            if (option == XframeOption.AllowFrom)
            {
                return "ALLOW-FROM " + allowFrom;
            }

            return option.ToString("G").ToUpperInvariant();
        }
    }

    public enum XframeOption
    {
        Deny = 0,
        SameOrigin = 1,
        AllowFrom = 2
    }
}