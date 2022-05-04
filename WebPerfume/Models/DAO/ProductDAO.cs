using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebPerfume.Models.EF;

namespace WebPerfume.Models.DAO
{
    public class ProductDAO
    {
        private RosePerfumeDBModel db = new RosePerfumeDBModel();

        public List<Product> ListAll()
        {
            return db.Products.ToList();
        }

        public List<Product> ViewDetailByNSXID(int id)
        {
            var model = from a in db.Products
                        join b in db.Categories
                        on a.CategoryId equals b.Id
                        where a.CategoryId == id

                        select a;
            return model.ToList();
        }

        public List<Product> ListAllByCategoryId(int id)
        {
            return db.Products.Where(n => n.CategoryId == id).ToList();
        }

        public List<string> ListName(string keyword)
        {
            return db.Products.Where(n => n.Name.Contains(keyword)).Select(n => n.Name).ToList();
        }

        public List<Product> Search(string keyword)
        {
            return db.Products.Where(n => n.Name.Contains(keyword)).ToList();
        }

        public List<Product> ListRelate(int id)
        {
            var idnsx = db.Products.Where(n => n.Id == id).Select(n => n.CategoryId).First();
            return db.Products.Where(n => n.CategoryId == idnsx).ToList();
        }

        public void setQuantity(int id, int? quantity)
        {
            Product product = db.Products.Find(id);
            product.Quantity -= quantity;
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
        }

        public List<Product> ListFeatured(int top)
        {
            return db.Products.Where(x => x.Featured == true).Take(top).ToList();
        }

        public IEnumerable<Product> ListAllPaging(string searchString, int page, int pagesize)
        {
            IQueryable<Product> model = db.Products;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(n => n.Name.Contains(searchString));
            }
            return model.OrderBy(n => n.Id).ToPagedList(page, pagesize);
        }

        public Product ViewDetail(int id)
        {
            return db.Products.Find(id);
        }

        public bool Delete(int id)
        {
            try
            {
                var sp = db.Products.Find(id);
                db.Products.Remove(sp);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Product> ListHotDeal(int top)
        {
            return db.Products.Where(n => n.TopHot == true).Take(top).ToList();
        }

        public List<Product> ListNewSamSung()
        {
            return db.Products.Where(n => n.CategoryId == 1).OrderByDescending(x => x.Id).Take(6).ToList();
        }

        public List<Product> ListNewSony()
        {
            return db.Products.Where(n => n.CategoryId == 2).OrderByDescending(x => x.Id).Take(6).ToList();
        }

        public List<Product> ListNewLG()
        {
            return db.Products.Where(n => n.CategoryId == 3).OrderByDescending(x => x.Id).Take(6).ToList();
        }

        public Product FeaturedProduct()
        {
            var prd = db.Products.Where(x => x.Id == 2).FirstOrDefault();
            return prd;
        }

        public List<Product> ListFeaturedSamSung()
        {
            return db.Products.Where(x => x.CategoryId == 1 && x.Featured == true).Take(6).ToList();
        }

        public List<Product> ListFeaturedSony()
        {
            return db.Products.Where(x => x.CategoryId == 2 && x.Featured == true).Take(6).ToList();
        }

        public List<Product> ListFeaturedLG()
        {
            return db.Products.Where(x => x.CategoryId == 3 && x.Featured == true).Take(6).ToList();
        }

        public List<Product> ListFeaturedToshiba()
        {
            return db.Products.Where(x => x.CategoryId == 4 && x.TopHot == true).Take(6).ToList();
        }

        public List<Product> ListRelated(int id)
        {
            int? categoryid = db.Products.Where(x => x.Id == id).Select(x => x.CategoryId).FirstOrDefault();
            var prd = db.Products.Where(x => x.CategoryId == categoryid && x.Id != id).ToList();
            return prd;
        }
    }
}