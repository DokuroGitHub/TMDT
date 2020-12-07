namespace Model.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User")]
    public partial class User
    {
        public long ID { get; set; }

        [StringLength(50)]
        [Display(Name = "User_UserName", ResourceType = typeof(StaticResources.Resources))]
        public string UserName { get; set; }

        [StringLength(32)]
        [Display(Name = "User_Password", ResourceType = typeof(StaticResources.Resources))]
        public string Password { get; set; }
//cần displayname
        public string GroupID { set; get; }

        [StringLength(50)]
        [Display(Name = "User_Name", ResourceType = typeof(StaticResources.Resources))]
        public string Name { get; set; }

        [StringLength(50)]
        [Display(Name = "User_Address", ResourceType = typeof(StaticResources.Resources))]
        public string Address { get; set; }

        [StringLength(50)]
        [Display(Name = "User_Email", ResourceType = typeof(StaticResources.Resources))]
        public string Email { get; set; }

        [StringLength(50)]
        [Display(Name = "User_Phone", ResourceType = typeof(StaticResources.Resources))]
        public string Phone { get; set; }

        [Display(Name = "User_ProvinceID", ResourceType = typeof(StaticResources.Resources))]
        public int? ProvinceID { set; get; }

        [Display(Name = "User_DistrictID", ResourceType = typeof(StaticResources.Resources))]
        public int? DistrictID { set; get; }

        [Display(Name = "User_CreatedDate", ResourceType = typeof(StaticResources.Resources))]
        public DateTime? CreatedDate { get; set; }

        [StringLength(50)]
        [Display(Name = "User_CreatedBy", ResourceType = typeof(StaticResources.Resources))]
        public string CreatedBy { get; set; }

        [Display(Name = "User_ModifiedDate", ResourceType = typeof(StaticResources.Resources))]
        public DateTime? ModifiedDate { get; set; }

        [StringLength(50)]
        [Display(Name = "User_ModifiedBy", ResourceType = typeof(StaticResources.Resources))]
        public string ModifiedBy { get; set; }

        [Display(Name = "User_Status", ResourceType = typeof(StaticResources.Resources))]
        public bool Status { get; set; }

        public string Language { get; set; }
    }
}
