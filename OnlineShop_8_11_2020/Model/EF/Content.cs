namespace Model.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Content")]
    public partial class Content
    {
        public long ID { get; set; }

        [StringLength(250)]
        [Display(Name = "Content_Name", ResourceType = typeof(StaticResources.Resources))]
        public string Name { get; set; }

        [StringLength(10)]
        [Display(Name = "Content_MetaTitle", ResourceType = typeof(StaticResources.Resources))]
        public string MetaTitle { get; set; }

        [StringLength(500)]
        [Display(Name = "Content_Description", ResourceType = typeof(StaticResources.Resources))]
        public string Description { get; set; }

        [StringLength(250)]
        [Display(Name = "Content_Image", ResourceType = typeof(StaticResources.Resources))]
        public string Image { get; set; }

        [Display(Name = "Content_CategoryID", ResourceType = typeof(StaticResources.Resources))]
        public long? CategoryID { get; set; }

        [Column(TypeName = "ntext")]
        [Display(Name = "Content_Detail", ResourceType = typeof(StaticResources.Resources))]
        public string Detail { get; set; }

        [Display(Name = "Content_Warranty", ResourceType = typeof(StaticResources.Resources))]
        public int? Warranty { get; set; }

        [Display(Name = "Content_CreatedDate", ResourceType = typeof(StaticResources.Resources))]
        public DateTime? CreatedDate { get; set; }

        [StringLength(50)]
        [Display(Name = "Content_CreatedBy", ResourceType = typeof(StaticResources.Resources))]
        public string CreatedBy { get; set; }

        [Display(Name = "Content_ModifiedDate", ResourceType = typeof(StaticResources.Resources))]
        public DateTime? ModifiedDate { get; set; }

        [StringLength(50)]
        [Display(Name = "Content_ModifiedBy", ResourceType = typeof(StaticResources.Resources))]
        public string ModifiedBy { get; set; }

        [StringLength(250)]
        [Display(Name = "Content_MetaKeywords", ResourceType = typeof(StaticResources.Resources))]
        public string MetaKeywords { get; set; }

        [StringLength(250)]
        [Display(Name = "Content_MetaDescriptions", ResourceType = typeof(StaticResources.Resources))]
        public string MetaDescriptions { get; set; }

        [Display(Name = "Content_Status", ResourceType = typeof(StaticResources.Resources))]
        public bool Status { get; set; }

        [Display(Name = "Content_TopHot", ResourceType = typeof(StaticResources.Resources))]
        public DateTime? TopHot { get; set; }

        [Display(Name = "Content_ViewCount", ResourceType = typeof(StaticResources.Resources))]
        public long? ViewCount { get; set; }

        [StringLength(500)]
        [Display(Name = "Content_Tags", ResourceType = typeof(StaticResources.Resources))]
        public string Tags { get; set; }

        public string Language { get; set; }
    }
}
