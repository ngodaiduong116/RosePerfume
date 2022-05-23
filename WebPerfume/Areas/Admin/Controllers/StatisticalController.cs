using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
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

        public ActionResult Index()
        {
            return View();
        }


        public JsonResult AjaxStatistical(DataSourceLoadOptions options)
        {
            var listOrder = db.Orders.Where(x => x.Status != EnumStatus.New && x.Status != EnumStatus.Pendding);
            return new JsonResult()
            {
                Data = DataSourceLoader.Load(listOrder, options),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue
            };
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