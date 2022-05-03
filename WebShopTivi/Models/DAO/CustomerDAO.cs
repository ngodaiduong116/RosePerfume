using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopTivi.Models.EF;

namespace WebShopTivi.Models.DAO
{
    public class CustomerDAO
    {
        ShopTiviDBModel db = new ShopTiviDBModel();
        public IEnumerable<Customer> ListAllPaging(string searchString, int page, int pagesize)
        {
            IQueryable<Customer> model = db.Customers;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(n => n.Name.Contains(searchString));
            }
            return model.OrderBy(n => n.Id).ToPagedList(page, pagesize);
        }
    }
}