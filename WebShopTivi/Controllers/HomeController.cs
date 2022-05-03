using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShopTivi.Models;
using WebShopTivi.Models.DAO;
using WebShopTivi.Models.EF;

namespace WebShopTivi.Controllers
{
    public class HomeController : Controller
    {
        private string CartSession = "CartSession";
        public ActionResult Index()
        {
            ViewBag.ListHotDeal = new ProductDAO().ListHotDeal(6);
            ViewBag.ListNewSamSung = new ProductDAO().ListNewSamSung();
            ViewBag.ListNewSony = new ProductDAO().ListNewSony();
            ViewBag.ListNewLG = new ProductDAO().ListNewLG();
            ViewBag.ListNews = new NewsDAO().ListHome(3);
            ViewBag.ListCategory = new CategoryDAO().ListName();
            
            ViewBag.FeaturedProduct = new ProductDAO().FeaturedProduct();
            ViewBag.ListFeaturedSamSung = new ProductDAO().ListFeaturedSamSung();
            ViewBag.ListFeaturedSony = new ProductDAO().ListFeaturedSony();
            ViewBag.ListFeaturedLG = new ProductDAO().ListFeaturedLG();
            ViewBag.ListFeaturedToshiba = new ProductDAO().ListFeaturedToshiba();
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