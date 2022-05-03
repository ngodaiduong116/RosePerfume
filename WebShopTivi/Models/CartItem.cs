using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopTivi.Models.EF;

namespace WebShopTivi.Models
{
    public class CartItem
    {
        public Product Product { get; set; }

        public int quantity { get; set; }
        public decimal? Total { get; set; }
    }
}