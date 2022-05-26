using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPerfume.Areas.Admin.Model;
using WebPerfume.Common;
using WebPerfume.Models.EF;

namespace WebPerfume.Areas.Admin.Controllers
{
    public class StaffController : Controller
    {
        private RosePerfumeDBModel db = new RosePerfumeDBModel();

        // GET: Admin/Staff
        public ActionResult Index()
        {
            //initialize a datetime variable with today
            DateTime today = DateTime.Today;

            //create a datetime object with first day of current month
            DateTime firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

            var data = from od in db.OrderDetails
                       join o in db.Orders on od.OrderId equals o.Id
                       join p in db.Products on od.ProductId equals p.Id
                       where o.Status != EnumStatus.Approved && o.CreateDate >= firstDayOfMonth && o.CreateDate <= today
                       select new
                       {
                           ImportPrice = p.ImportPrice,
                           Price = od.Price,
                           Quantity = od.Quantity,
                           ProductId = p.Id
                       };
            var revenue = data.Sum(x => x.Quantity * x.Price);
            var import = data.Sum(x => x.Quantity * x.ImportPrice);
            var listOrder = db.Orders.Where(x => x.Status != EnumStatus.New && x.Status != EnumStatus.Pendding);

            var getListIdOrderDone = listOrder.Where(x => x.Status == EnumStatus.Approved).Select(x => x.Id).ToList();
            var getListIdOrderFalse = listOrder.Where(x => x.Status == EnumStatus.Cancel || x.Status == EnumStatus.Reject).Select(x => x.Id).ToList();

            var totalRevenueQuantityDone = db.OrderDetails.Where(x => getListIdOrderDone.Contains(x.OrderId)).Sum(x => x.Quantity);
            var totalRevenueQuantityFalse = db.OrderDetails.Where(x => getListIdOrderFalse.Contains(x.OrderId)).Sum(x => x.Quantity);

            ViewBag.TotalRevenueQuantityDone = totalRevenueQuantityDone;
            ViewBag.TotalRevenueQuantityFalse = totalRevenueQuantityFalse;
            ViewBag.Revenue = revenue;
            ViewBag.Profit = revenue - import;
            return View();
        }

        public JsonResult AjaxStaff(DataSourceLoadOptions options, DateTime dateFrom, DateTime dateTo)
        {
            List<Order> listOrder = db.Orders.Where(x => x.Status != EnumStatus.New && x.CreateDate <= dateTo && x.CreateDate >= dateFrom).OrderByDescending(x => x.CreateDate).ToList();
            List<CartModel> listData = new List<CartModel>();
            listOrder.ForEach(item =>
            {
                var obj = new CartModel();
                obj.Id = item.Id;
                obj.ShipName = item.ShipName;
                obj.ShipMobile = item.ShipMobile;
                obj.ShipEmail = item.ShipEmail;
                obj.ShipAddress = item.ShipAddress;
                obj.ShipSuccess = item.ShipSuccess;
                obj.CreateDate = item.CreateDate;
                obj.Status = item.Status;
                listData.Add(obj);
            });
            return new JsonResult()
            {
                Data = DataSourceLoader.Load(listData, options),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
            };
        }
        [HttpPost]
        public ActionResult CheckDate(DateTime dateFrom, DateTime dateTo)
        {
            var data = from od in db.OrderDetails
                       join o in db.Orders on od.OrderId equals o.Id
                       join p in db.Products on od.ProductId equals p.Id
                       where o.Status != EnumStatus.Approved && o.CreateDate >= dateFrom && o.CreateDate <= dateTo
                       select new
                       {
                           ImportPrice = p.ImportPrice,
                           Price = od.Price,
                           Quantity = od.Quantity,
                           ProductId = p.Id
                       };
            var revenue = data.Sum(x => x.Quantity * x.Price);
            var import = data.Sum(x => x.Quantity * x.ImportPrice);
            var listOrder = db.Orders.Where(x => x.Status != EnumStatus.New && x.Status != EnumStatus.Pendding && x.CreateDate >= dateFrom && x.CreateDate <= dateTo);

            var getListIdOrderDone = listOrder.Where(x => x.Status == EnumStatus.Approved).Select(x => x.Id).ToList();
            var getListIdOrderFalse = listOrder.Where(x => x.Status == EnumStatus.Cancel || x.Status == EnumStatus.Reject).Select(x => x.Id).ToList();

            var totalRevenueQuantityDone = db.OrderDetails.Where(x => getListIdOrderDone.Contains(x.OrderId)).Sum(x => x.Quantity);
            var totalRevenueQuantityFalse = db.OrderDetails.Where(x => getListIdOrderFalse.Contains(x.OrderId)).Sum(x => x.Quantity);

            ViewBag.TotalRevenueQuantityDone = totalRevenueQuantityDone;
            ViewBag.TotalRevenueQuantityFalse = totalRevenueQuantityFalse;
            ViewBag.Revenue = revenue;
            ViewBag.Profit = revenue - import;
            return Json(new
            {
                TotalRevenueQuantityDone = totalRevenueQuantityDone,
                TotalRevenueQuantityFalse = totalRevenueQuantityFalse,
                Revenue = revenue,
                Profit = revenue - import
            });
        }
    }
}