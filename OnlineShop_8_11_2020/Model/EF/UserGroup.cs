using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Model.EF
{
    [Table("UserGroup")]
    public class UserGroup
    {
        [StringLength(20)]
        public string ID { get; set; }

        [StringLength(250)]
        public string Name { get; set; }
    }
}
