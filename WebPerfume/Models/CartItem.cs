using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebPerfume.Models.EF;

namespace WebPerfume.Models
{
    public class CartItem
    {
        public string NameCustomer { get; set; }
        public string NameProduct { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
        public decimal TotalMoney { get; set; }
    }
}