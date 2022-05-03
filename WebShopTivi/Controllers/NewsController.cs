using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShopTivi.Models.DAO;
using WebShopTivi.Models.EF;

namespace WebShopTivi.Controllers
{
    public class NewsController : Controller
    {
        // GET: News
        public ActionResult Index(int page = 1, int pageSize = 16)
        {
            var model = new NewsDAO().ListAll(page, pageSize);
            ViewBag.ListRecently = new NewsDAO().ListRecently();
            ViewBag.ListTag = new NewsDAO().ListTag();
            return View(model);
        }

        public ActionResult Detail(int id)
        {
            var news = new NewsDAO().ViewDetail(id);
            return View(news);
        }
        
        public ActionResult SearchNews(string searchString, int page = 1, int pageSize = 16)
        {
            var model = new NewsDAO().ListSearch(searchString, page, pageSize);
            return View(model);
        }
    }
}