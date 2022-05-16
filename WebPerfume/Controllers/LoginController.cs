using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPerfume.Models.EF;

namespace WebPerfume.Controllers
{
    public class LoginController : Controller
    {
        RosePerfumeDBModel db = new RosePerfumeDBModel();
        // GET: Login
        public ActionResult Index()
        {
            if (!Session["UserClient"].Equals("") && !Session["UserClientUsername"].Equals(""))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection field)
        {
            RosePerfumeDBModel db = new RosePerfumeDBModel();
            string strerr = "";
            string username = field["username"];
            string password = field["password"];

            var rowuser = db.Customers.Where(n => n.Username == username).FirstOrDefault();
            if (rowuser == null)
            {
                strerr = "Không tồn tại tài khoản";
            }
            else
            {
                if (rowuser.Password.Equals(password))
                {
                    Session["UserClient"] = rowuser.Name;
                    Session["UserClientUsername"] = rowuser.Username;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    strerr = "Mật khẩu không đúng";
                }
            }

            ViewBag.Error = "<span class='text-danger'>" + strerr + "</span>";
            return View();
        }

        public ActionResult Logout()
        {
            Session["UserClient"] ="";
            Session["UserClientUsername"] = "";
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Customer customer)
        {           
            var model = new Customer();
            model.Name = customer.Name;
            if(db.Customers.Any(x=>x.Username == customer.Username))
            {
                ViewBag.Error = "<span class='text-danger'>" + "Tài khoản này đã tồn tại" + "</span>";
                return View();
            }
            else
            {
                model.Username = customer.Username;
                model.Password = customer.Password;
                model.Email = customer.Email;
                model.Address = customer.Address;
                model.Mobile = customer.Mobile;
                model.Status = true;

                db.Customers.Add(model);
                db.SaveChanges();

                Session["UserClient"] = model.Name;
                Session["UserClientUsername"] = model.Username;
                return RedirectToAction("Index", "Home");
            }
            
        }
    }
}