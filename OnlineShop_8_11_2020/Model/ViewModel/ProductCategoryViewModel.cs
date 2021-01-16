using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class ProductCategoryViewModel
    {
        //đang fix
        public long ID { set; get; }
        public string Name { set; get; }
        public string MetaName { set; get; }
        //public int? Status { set; get; }
        public long TotalProducts { set; get; }
        public decimal TotalCost { set; get; }

        public long TotalProductsSold { set; get; }
        public decimal TotalSold { set; get; }
    }
}
