namespace EcommerceWebApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {
        [Key]
        public int ProdID { get; set; }

        [Required]
        [StringLength(50)]
        public string Productname { get; set; }

        public int price { get; set; }

        [StringLength(350)]
        public string description { get; set; }

        public int catId { get; set; }

        [StringLength(150)]
        public string ProductPic { get; set; }

        public virtual Category Category { get; set; }
    }
}
