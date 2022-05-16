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
        RosePerfumeDBModel db = new RosePerfumeDBModel();
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
            var listResult = new List<Cart>();
            if ((string)Session["UserClientUsername"] != "")
            {
                var userCurrent = (string)Session["UserClientUsername"].ToString();
                var getCus = new CustomerDAO().getCustomer(userCurrent);
                var listProductInCart = db.Carts.Where(x => x.CustomerId == getCus.Id).ToList();
                listResult = listProductInCart;
                ViewBag.SoLuong = listProductInCart.Count;
            }
            else
            {
                var getCartOfSession = Session[CartSession];
                if (getCartOfSession != null)
                {
                    listResult = (List<Cart>)getCartOfSession;
                    ViewBag.SoLuong = ((List<Cart>)getCartOfSession).Count;
                }
                else
                {
                    ViewBag.SoLuong = 0;
                }
            }
            return PartialView(listResult);
        }
    }
}