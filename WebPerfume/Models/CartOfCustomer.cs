using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebPerfume.Models.EF;

namespace WebPerfume.Models
{
    public class CartOfCustomer
    {
        public int STT { get; set; }
        public int OrderId { get; set; }
        public DateTime Created { get; set; }
        public decimal totalBill { get; set; }
        public int quantity { get; set; }
        public int Stautus { get; set; }
    }
}