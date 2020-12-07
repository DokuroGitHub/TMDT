using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class CouponItem
    {
        public long CouponID { set; get; }
        public string Name { set; get; }
        public string Code { set; get; }
        public bool ByPercentage { set; get; }
        public decimal DiscountBy { set; get; }
        public decimal DiscountAmount { set; get; }
    }
}
