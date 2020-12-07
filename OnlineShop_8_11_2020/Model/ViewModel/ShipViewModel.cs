using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class ShipViewModel
    {
        //đang fix

        //thông tin shipper cần xem
        public long OrderID { set; get; }
        public long? CustomerID { set; get; }
        public string ShipToName { set; get; }
        public string ShipMobile { set; get; }
        public string ShipAddress { set; get; } //địa điểm ship tới
        public string ShipEmail { set; get; }

        public List<ShipItem> ShipItemList { set; get; }
        public List<CouponItem> CouponList { set; get; }

        public long? ShipperID { set; get; } //ID shipper
        public string ShipperName { set; get; } //tên shipper

        public int? Status { set; get; } //trạng thái đơn hàng(chưa ship, đang ship, đã ship, hủy)
    }
}
