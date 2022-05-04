using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebPerfume.Models.EF;

namespace WebPerfume.Models
{
    public class CartItem
    {
        public Product Product { get; set; }

        public int quantity { get; set; }
        public decimal? Total { get; set; }
    }
}