using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebPerfume.Models;
using WebPerfume.Models.EF;

namespace WebPerfume.Controllers
{
    public class CustomerController : Controller
    {
        private RosePerfumeDBModel db = new RosePerfumeDBModel();

        public ActionResult Index()
        {
            var listResult = new List<CartOfCustomer>();
            var userCurrent = (string)Session["UserClientUsername"].ToString();
            var getCus = db.Customers.FirstOrDefault(x => x.Username == userCurrent);
            var getOrderByCus = db.Orders.Where(x => x.CustomerId == getCus.Id).OrderByDescending(x => x.CreateDate).ToList();
            if (getOrderByCus.Count != 0)
            {
                int index = 1;
                getOrderByCus.ForEach(item =>
                {
                    var obj = new CartOfCustomer();
                    var getListProduct = db.OrderDetails.Where(x => x.OrderId == item.Id).ToList();
                    obj.STT = index;
                    obj.OrderId = item.Id;
                    obj.Created = item.CreateDate;
                    obj.totalBill = getListProduct.Sum(x => x.TotalMoney);
                    obj.quantity = getListProduct.Sum(x => x.Quantity);
                    obj.Stautus = item.Status;
                    listResult.Add(obj);
                    index++;
                });
            }
            ViewBag.Customer = getCus;
            return View(listResult);
        }

        public ActionResult Details(int id)
        {
            Order order = db.Orders.Find(id);
            return View(db.OrderDetails.Where(x => x.OrderId == order.Id).ToList());
        }

        public ActionResult ChangePassword()
        {
            var userCurrent = (string)Session["UserClientUsername"].ToString();
            var getCus = db.Customers.FirstOrDefault(x => x.Username == userCurrent);
            var result = new ChangePaswordModel();
            result.Id = getCus.Id;
            result.Name = getCus.Name;
            return View(result);
        }
    }
}