using System.Web.Mvc;

namespace OnlineShop.Areas.Shipper
{
    public class ShipperAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Shipper";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Shipper_default",
                "Shipper/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new { controller = "Base|Home|Login|Ship" },
                new[] { "OnlineShop.Areas.Shipper.Controllers" }
            );
        }
    }
}