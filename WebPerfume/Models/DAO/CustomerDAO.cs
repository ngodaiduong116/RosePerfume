using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebPerfume.Models.EF;

namespace WebPerfume.Models.DAO
{
    public class CustomerDAO
    {
        RosePerfumeDBModel db = new RosePerfumeDBModel();
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