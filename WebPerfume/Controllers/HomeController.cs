using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPerfume.Models;
using WebPerfume.Models.DAO;
using WebPerfume.Models.EF;

namespace WebPerfume.Controllers
{
    public class HomeController : Controller
    {
        private string CartSession = "CartSession";
        public ActionResult Index()
        {            
            ViewBag.ListCategory = new CategoryDAO().ListName();
            ViewBag.FeaturedProduct = new ProductDAO().FeaturedProduct();
            ViewBag.ListHotDeal = new ProductDAO().ListHotDeal(6);
            ViewBag.ListNews = new NewsDAO().ListHome(3);

            ViewBag.ListNewChanel = new ProductDAO().ListNewChanel();
            ViewBag.ListNewDior = new ProductDAO().ListNewDior();
            ViewBag.ListNewCalvinKlein = new ProductDAO().ListNewCalvinKlein();
            ViewBag.ListFeaturedGucci = new ProductDAO().ListFeaturedGucci();
            ViewBag.ListFeaturedDiory = new ProductDAO().ListFeaturedDiory();
            ViewBag.ListFeaturedCalvinKlein = new ProductDAO().ListFeaturedCalvinKlein();

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult Menu()
        {
            var model = new CategoryDAO().ListName();
            return PartialView(model);
        }

        [ChildActionOnly]
        public PartialViewResult HeaderCart()
        {
            var cart = Session[CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
                ViewBag.SoLuong = list.Count;
            }
            else
            {
                ViewBag.SoLuong = 0;
            }
            return PartialView(list);
        }
    }
}