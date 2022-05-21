using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPerfume.Common;
using WebPerfume.Models;
using WebPerfume.Models.EF;

namespace WebPerfume.Areas.Admin.Controllers
{
    public class StatisticalController : BaseController
    {
        private RosePerfumeDBModel db = new RosePerfumeDBModel();

        public ActionResult Index(FormCollection fields)
        {
            var data = from od in db.OrderDetails
                       join o in db.Orders on od.OrderId equals o.Id
                       join p in db.Products on od.ProductId equals p.Id
                       where o.Status == EnumStatus.Approved
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
            var getListIdOrderFalse = listOrder.Where(x => x.Status != EnumStatus.Approved).Select(x => x.Id).ToList();

            var totalRevenueQuantityDone = db.OrderDetails.Where(x => getListIdOrderDone.Contains(x.OrderId)).Sum(x => x.Quantity);
            var totalRevenueQuantityFalse = db.OrderDetails.Where(x => getListIdOrderFalse.Contains(x.OrderId)).Sum(x => x.Quantity);

            ViewBag.ListOrder = listOrder;
            ViewBag.TotalRevenueQuantityDone = totalRevenueQuantityDone;
            ViewBag.TotalRevenueQuantityFalse = totalRevenueQuantityFalse;
            ViewBag.Revenue = revenue;
            ViewBag.Profit = revenue - import;
            return View();
        }

        [HttpPost]
        public ActionResult ThongKe(FormCollection fields)
        {
            string ngaybatdau = fields["ngaybatdau"];
            string ngayketthuc = fields["ngayketthuc"];

            DateTime nbd = DateTime.Parse(ngaybatdau);
            DateTime nkt = DateTime.Parse(ngayketthuc);

            var data = from od in db.OrderDetails
                       join o in db.Orders on od.OrderId equals o.Id
                       join p in db.Products on od.ProductId equals p.Id
                       where o.Status == EnumStatus.Approved && o.CreateDate >= nbd && o.CreateDate <= nkt
                       select new
                       {
                           ImportPrice = p.ImportPrice,
                           Price = od.Price,
                           Quantity = od.Quantity,
                           ProductId = p.Id
                       };
            var revenue = data.Sum(x => x.Quantity * x.Price);
            var import = data.Sum(x => x.Quantity * x.ImportPrice);
            var listOrder = db.Orders.Where(x => x.Status != EnumStatus.New && x.Status != EnumStatus.Pendding && x.CreateDate >= nbd && x.CreateDate <= nkt).ToList();

            var getListIdOrderDone = listOrder.Where(x => x.Status == EnumStatus.Approved).Select(x => x.Id).ToList();
            var getListIdOrderFalse = listOrder.Where(x => x.Status != EnumStatus.Approved).Select(x => x.Id).ToList();

            var totalRevenueQuantityDone = db.OrderDetails.Where(x => getListIdOrderDone.Contains(x.OrderId)).Sum(x => x.Quantity);
            var totalRevenueQuantityFalse = db.OrderDetails.Where(x => getListIdOrderFalse.Contains(x.OrderId)).Sum(x => x.Quantity);

            ViewBag.ListOrder = listOrder;
            ViewBag.TotalRevenueQuantityDone = totalRevenueQuantityDone;
            ViewBag.TotalRevenueQuantityFalse = totalRevenueQuantityFalse;
            ViewBag.Revenue = revenue;
            ViewBag.Profit = revenue - import;
            return View("Index");
        }

        //public JsonResult Chart(int year)
        //{
        //    List<decimal> price = new List<decimal>();
        //    List<decimal> stock = new List<decimal>();
        //    Dictionary<string, List<decimal>> result = new Dictionary<string, List<decimal>>();

        //    for (int month = 1; month <= 12; month++)
        //    {
        //        var priceByMonth = db.OrderDetails
        //            .Where(x => x.Order.Status == true && x.Order.CreateDate.Value.Month == month && x.Order.CreateDate.Value.Year == year)
        //            .Sum(x => x.Quantity * x.Price);

        //        price.Add(priceByMonth == null ? 0 : priceByMonth.Value);
        //        var stockByMonth = (from od in db.OrderDetails
        //                            join p in db.Products on od.ProductId equals p.Id
        //                            join o in db.Orders on od.OrderId equals o.Id
        //                            where o.Status == true && od.Order.CreateDate.Value.Year == year && od.Order.CreateDate.Value.Month == month
        //                            select new
        //                            {
        //                                Quantity = od.Quantity,
        //                                Stock = p.ImportPrice
        //                            }).Sum(x => x.Quantity * x.Stock);
        //        stock.Add(stockByMonth == null ? 0 : stockByMonth.Value);
        //    }
        //    result.Add("price", price);
        //    result.Add("stock", stock);
        //    return Json(new { data = result },
        //        JsonRequestBehavior.AllowGet);
        //}
    }
}