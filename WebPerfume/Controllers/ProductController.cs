using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPerfume.Models.DAO;

namespace WebPerfume.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Detail(int id)
        {
            var model = new ProductDAO().ViewDetail(id);

            ViewBag.ListRelated = new ProductDAO().ListRelated(id);
            ViewBag.ListHot = new ProductDAO().ListHotDeal(6);
            return View(model);
        }

        public JsonResult ListName(string q)
        {
            var data = new ProductDAO().ListName(q);
            return Json(new
            {
                data = data,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchProduct(string keyword)
        {
            var model = new ProductDAO().Search(keyword);
            ViewBag.Keyword = keyword;
            return View(model);
        }
        
    }
}