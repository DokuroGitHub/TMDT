using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Model.EF
{
    [Serializable]
    [Table("Credential")]
    public class Credential
    {
        [Key]
        [StringLength(20)]
        public string UserGroupID { set; get; }

        [StringLength(50)]
        public string RoleID { set; get; }
    }
}
