using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebPerfume.Models.EF;

namespace WebPerfume.Models.DAO
{
    public class NewsDAO
    {
        RosePerfumeDBModel db = new RosePerfumeDBModel();
        public List<News> ListAll()
        {
            return db.News.ToList();
        }
        public IEnumerable<News> ListAll(int page, int pageSize)
        {
            IQueryable<News> model = db.News;
            
            return model.OrderBy(n => n.Id).ToPagedList(page, pageSize);
        }

        public IEnumerable<News> ListSearch(string searchString, int page, int pagesize)
        {
            IQueryable<News> model = db.News;
            var tagid = db.Tags.Where(x => x.Name == searchString).Select(x => x.Id).FirstOrDefault();
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.TagId == tagid);
            }
            return model.OrderBy(n => n.Id).ToPagedList(page, pagesize);
        }

        public IEnumerable<News> ListAllPaging(string searchString, int page, int pagesize)
        {
            IQueryable<News> model = db.News;
            var tagid = db.Tags.Where(x => x.Name == searchString).Select(x => x.Id).FirstOrDefault();
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.TagId == tagid);
            }
            return model.OrderBy(n => n.Id).ToPagedList(page, pagesize);
        }

        public List<News> ListRecently()
        {
            return db.News.OrderByDescending(x => x.Id).Take(5).ToList();
        }

        public List<Tag> ListTag()
        {
            return db.Tags.Take(9).ToList();
        }


        public News ViewDetail(int id)
        {
            return db.News.Find(id);
        }

        public bool Delete(int id)
        {
            try
            {
                var news = db.News.Find(id);
                db.News.Remove(news);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<News> ListHome(int top)
        {
            return db.News.Take(top).ToList();
        }
    }
}