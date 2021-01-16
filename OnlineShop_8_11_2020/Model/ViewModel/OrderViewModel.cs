using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class OrderViewModel
    {
        //đang fix
        public long OrderID { set; get; }
        public DateTime? CreatedDate { set; get; }
        public int? Status { set; get; } //trạng thái đơn hàng(chưa ship, đang ship, đã ship, hủy)
        public decimal TotalPrice { set; get; } //trạng thái đơn hàng(chưa ship, đang ship, đã ship, hủy)
    }
}
