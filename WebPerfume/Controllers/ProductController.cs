using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPerfume.Models.DAO;
using WebPerfume.Models.EF;

namespace WebPerfume.Controllers
{
    public class ProductController : Controller
    {
        private RosePerfumeDBModel db = new RosePerfumeDBModel();
        private string CartSession = "CartSession";

        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail(int id)
        {
            int QuantityProductInCart = 0;
            if ((string)Session["UserClientUsername"] != "")
            {
                var userCurrent = (string)Session["UserClientUsername"].ToString();
                var getCus = db.Customers.FirstOrDefault(x => x.Username == userCurrent);
                var getProductInCart = db.Carts.FirstOrDefault(x => x.CustomerId == getCus.Id && x.ProductId == id);
                if (getProductInCart != null)
                {
                    QuantityProductInCart = getProductInCart.Quantity;
                }
            }
            else
            {
                var listProductOfCart = (List<Cart>)Session[CartSession];
                bool check = listProductOfCart.Exists(x => x.ProductId == id);
                if (listProductOfCart.Exists(x => x.ProductId == id))
                {
                    QuantityProductInCart = listProductOfCart.Find(x => x.ProductId == id).Quantity;
                }
            }

            var model = new ProductDAO().ViewDetail(id);
            ViewBag.ListRelated = new ProductDAO().ListRelated(id);
            ViewBag.ListHot = new ProductDAO().ListHotDeal(6);
            ViewBag.QuantityProductInCart = QuantityProductInCart;
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