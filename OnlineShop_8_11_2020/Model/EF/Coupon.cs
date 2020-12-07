using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;


namespace Model.EF
{
    [Table("Coupon")]
    public class Coupon
    {
        [Key]
        public long ID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        public long? Quantity { get; set; }

        public bool ByPercentage { get; set; }

        public decimal DiscountBy { get; set; }

        public bool Status { get; set; }
    }
}
