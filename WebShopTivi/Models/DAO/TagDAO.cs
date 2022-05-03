using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopTivi.Models.EF;

namespace WebShopTivi.Models.DAO
{
    public class TagDAO
    {
        RosePerfumeDBModel db = new RosePerfumeDBModel();
        public IEnumerable<Tag> ListAllPaging(string searchString, int page, int pagesize)
        {
            IQueryable<Tag> model = db.Tags;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(n => n.Name.Contains(searchString));
            }
            return model.OrderBy(n => n.Id).ToPagedList(page, pagesize);
        }

        public List<Tag> ListName()
        {
            return db.Tags.ToList();
        }
    }
}