using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebPerfume.Models.EF;

namespace WebPerfume.Models.DAO
{
    public class CategoryDAO
    {
        RosePerfumeDBModel db = new RosePerfumeDBModel();
        public IEnumerable<Category> ListAllPaging(string searchString, int page, int pagesize)
        {
            IQueryable<Category> model = db.Categories;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(n => n.Name.Contains(searchString));
            }
            return model.OrderBy(n => n.Id).ToPagedList(page, pagesize);
        }
        public int Insert(Category entity, string path)
        {
            var category = new Category();
            category.Name = entity.Name;
            category.Image = path;
            category.CreateDate = DateTime.Now;
            category.Status = entity.Status;
            db.Categories.Add(category);
            db.SaveChanges();
            return entity.Id;
        }

        public Category ViewDetail(int id)
        {
            return db.Categories.Find(id);
        }

        public bool Update(Category entity, string path)
        {
            try
            {
                var category = db.Categories.Find(entity.Id);
                category.Name = entity.Name;
                if(path == "-1")
                {
                    category.Image = entity.Image;
                }
                else
                {
                    category.Image = path;
                }
                
                category.Status = entity.Status;
                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                var taikhoan = db.Categories.Find(id);
                db.Categories.Remove(taikhoan);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<Category> ListName()
        {
            return db.Categories.ToList();
        }

        
    }
}


