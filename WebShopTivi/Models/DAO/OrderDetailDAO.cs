using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopTivi.Models.EF;

namespace WebShopTivi.Models.DAO
{
    public class OrderDetailDAO
    {
        ShopTiviDBModel db = new ShopTiviDBModel();
        public bool Insert(OrderDetail detail)
        {
            try
            {
                db.OrderDetails.Add(detail);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}