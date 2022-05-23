using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebPerfume.Areas.Admin.Model
{
    public class CartModel
    {
        public string ShipName { get; set; }
        public string ShipMobile { get; set; }
        public string ShipEmail { get; set; }
        public string ShipAddress { get; set; }
        public bool ShipSuccess { get; set; }
        public int Status { get; set; }
    }
}