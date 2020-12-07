using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class ContentViewModel
    {
        public long ID { set; get; }
        public string Name { set; get; }
        public string MetaTitle { set; get; }
        public string Description { set; get; }
        public string Image { set; get; }

        public long? CategoryID { set; get; }
        public string CateName { set; get; }
        public string CateMetaTitle { set; get; }

        public string Detail { set; get; }
        public DateTime? CreatedDate { set; get; }
        public string CreatedBy { set; get; }
        public DateTime? ModifiedDate { set; get; }
        public string ModifiedBy { set; get; }
        public string MetaKeywords { set; get; }
        public string MetaDescriptions { set; get; }
        public long? ViewCount { set; get; }
        public string Tags { set; get; }
        public string Language { set; get; }
    }
}
