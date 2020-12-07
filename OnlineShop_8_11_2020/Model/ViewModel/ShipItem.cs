using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class ShipItem
    {
        public long ProductID { set; get; }
        public string Image { set; get; }
        public string Name { set; get; }
        public string MetaTitle { set; get; }
        public decimal Price { set; get; }
        public long Quantity { set; get; }
        public decimal TotalPrice { set; get; }
    }
}
