using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPerfume.Models.DAO;
using WebPerfume.Models.EF;

namespace WebPerfume.Controllers
{
    public class CategoryController : Controller
    {
        RosePerfumeDBModel db = new RosePerfumeDBModel();
        // GET: Category
        public ActionResult Index(int id, string SortOrder, int page = 1, int pageSize = 16)
        {
            ViewBag.CategoryId = id;
            ViewBag.CategoryName = db.Categories.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefault();

            ViewBag.CurrentSort = SortOrder;

            ViewBag.Newest = SortOrder == "New" ? "new_desc" : "New";
            ViewBag.LowToHigh = SortOrder == "Price" ? "price_desc" : "Price";
            ViewBag.HighToLow = SortOrder == "price_desc" ? "Price" : "price_desc";

            IEnumerable<Product> model = db.Products.Where(x => x.CategoryId == id);

            switch (SortOrder)
            {
                case "New":
                    model = model.OrderBy(s => s.CreateDate).ToPagedList(page, pageSize);
                    break;
                case "new_desc":
                    model = model.OrderByDescending(s => s.CreateDate).ToPagedList(page, pageSize);
                    break;
                case "Price":
                    model = model.OrderBy(s => s.Price).ToPagedList(page, pageSize);
                    break;
                case "price_desc":
                    model = model.OrderByDescending(s => s.Price).ToPagedList(page, pageSize);
                    break;
                
                default:
                    model = model.OrderBy(s => s.Name).ToPagedList(page, pageSize);
                    break;
            }

            return View(model);
        }
    }
}