using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShopTivi.Models.EF;

namespace WebShopTivi.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        private RosePerfumeDBModel db = new RosePerfumeDBModel();

        // GET: Admin/Home
        public ActionResult Index()
        {
            ViewBag.SoTaiKhoan = db.Accounts.Count();
            ViewBag.SoKhachHang = db.Customers.Count();
            ViewBag.SoSanPham = db.Products.Count();
            ViewBag.SoDanhMuc = db.Categories.Count();
            return View();
        }

        public ActionResult Info()
        {
            var name = Session["UserAdmin"];
            var userAdmin = db.Accounts.Where(x => x.Username == name.ToString()).FirstOrDefault();
            return View(userAdmin);
        }

        public ActionResult Logout()
        {
            Session["UserAdmin"] = "";
            Session["ImageAdmin"] = "";
            return RedirectToAction("Index", "Login");
        }
    }
}