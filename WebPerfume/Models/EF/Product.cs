namespace WebPerfume.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }

        public int? CategoryId { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        [StringLength(150)]
        public string Image { get; set; }

        [StringLength(150)]
        public string Image2 { get; set; }

        public decimal? ImportPrice { get; set; }

        public decimal? Price { get; set; }

        public decimal? PromotionPrice { get; set; }

        public double? PercentPromotion { get; set; }

        public string Description { get; set; }

        public string DescriptionDetail { get; set; }

        public int Volume { get; set; }
        public int ReleaseYear { get; set; }
        public string Style { get; set; }
        public string Sex { get; set; }
        public string Origin { get; set; }
        public bool TopHot { get; set; }

        public bool Featured { get; set; }

        public int? Quantity { get; set; }

        [StringLength(50)]
        public string CreateBy { get; set; }

        public DateTime? CreateDate { get; set; }

        public bool Status { get; set; }

        public virtual Category Category { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}