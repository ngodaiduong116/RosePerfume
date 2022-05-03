using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShopTivi.Models.EF;

namespace WebShopTivi.Areas.Admin.Controllers
{
    public class StatisticalController : BaseController
    {
        ShopTiviDBModel db = new ShopTiviDBModel();
        // GET: Admin/Statistical
        public ActionResult Index(FormCollection fields)
        {
            var khanh = from od in db.OrderDetails
                        join o in db.Orders on od.OrderId equals o.Id
                        join p in db.Products on od.ProductId equals p.Id
                        where o.Status == true
                        select new
                        {
                            ImportPrice = p.ImportPrice,
                            Price = od.Price,
                            Quantity = od.Quantity,
                            ProductId = p.Id
                        };
            var revenue = khanh.Sum(x => x.Quantity * x.Price);
            var import = khanh.Sum(x => x.Quantity * x.ImportPrice);
            ViewBag.Revenue = revenue;
            ViewBag.Profit = revenue - import;            

            //ViewBag.Revenue = db.OrderDetails.Sum(i => i.Price * i.Quantity);
            //var result = from o in db.OrderDetails
            //             join p in db.Products on o.ProductId equals p.Id
            //             select new
            //             {
            //                 ImportPrice = p.ImportPrice,
            //                 Price = o.Price,
            //                 Quantity = o.Quantity,
            //                 ProductId = p.Id
            //             };
            //var sumPrice = result.Sum(x => x.Quantity * x.Price);
            //var sum = result.Sum(x => x.Quantity * x.ImportPrice);
            //ViewBag.Profit = sumPrice - sum;
            return View();
        }

        [HttpPost]
        public ActionResult ThongKe(FormCollection fields)
        {
            string ngaybatdau = fields["ngaybatdau"];
            string ngayketthuc = fields["ngayketthuc"];

            DateTime nbd = DateTime.Parse(ngaybatdau);
            DateTime nkt = DateTime.Parse(ngayketthuc);

            var result = from od in db.OrderDetails
                         join o in db.Orders on od.OrderId equals o.Id
                         join p in db.Products on od.ProductId equals p.Id
                         where o.Status == true && o.CreateDate >= nbd && o.CreateDate <= nkt
                         select new
                         {
                             ImportPrice = p.ImportPrice,
                             Price = od.Price,
                             Quantity = od.Quantity,
                             ProductId = p.Id
                         };
            var revenue = result.Sum(x => x.Quantity * x.Price);
            var import = result.Sum(x => x.Quantity * x.ImportPrice);
            ViewBag.Revenue = revenue;
            ViewBag.Profit = revenue - import;
            return View("Index");
        }

        public JsonResult Chart(int year)
        {
            List<decimal> price = new List<decimal>();
            List<decimal> stock = new List<decimal>();
            Dictionary<string, List<decimal>> result = new Dictionary<string, List<decimal>>();

            for (int month = 1; month <= 12; month++)
            {
                var priceByMonth = db.OrderDetails
                    .Where(x => x.Order.Status == true && x.Order.CreateDate.Value.Month == month && x.Order.CreateDate.Value.Year == year)
                    .Sum(x => x.Quantity * x.Price);

                price.Add(priceByMonth == null ? 0 : priceByMonth.Value);
                var stockByMonth = (from od in db.OrderDetails
                                    join p in db.Products on od.ProductId equals p.Id
                                    join o in db.Orders on od.OrderId equals o.Id
                                    where o.Status == true && od.Order.CreateDate.Value.Year == year && od.Order.CreateDate.Value.Month == month
                                    select new
                                    {
                                        Quantity = od.Quantity,
                                        Stock = p.ImportPrice
                                    }).Sum(x => x.Quantity * x.Stock);
                stock.Add(stockByMonth == null ? 0 : stockByMonth.Value);
            }
            result.Add("price", price);
            result.Add("stock", stock);
            return Json(new { data = result },
                JsonRequestBehavior.AllowGet);
        }
    }
}