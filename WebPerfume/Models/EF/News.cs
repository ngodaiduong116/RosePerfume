namespace WebPerfume.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class News
    {
        public int Id { get; set; }

        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(150)]
        public string Image { get; set; }

        public string Description { get; set; }

        public string DescriptionDetail { get; set; }

        [StringLength(50)]
        public string CreateBy { get; set; }

        public DateTime? CreateDate { get; set; }

        public int? TagId { get; set; }

        public bool Status { get; set; }

        public virtual Tag Tag { get; set; }
    }
}
