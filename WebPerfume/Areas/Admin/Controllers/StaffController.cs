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
            return View();
        }
        public JsonResult AjaxStaff(DataSourceLoadOptions options)
        {
            List<Order> listOrder = db.Orders.Where(x => x.Status != EnumStatus.New && x.Status != EnumStatus.Pendding).ToList();
            List<CartModel> listData = new List<CartModel>();
            listOrder.ForEach(item => {
                var obj = new CartModel();
                obj.ShipName = item.ShipName;
                obj.ShipMobile = item.ShipMobile;
                obj.ShipEmail = item.ShipEmail;
                obj.ShipAddress = item.ShipAddress;
                obj.ShipSuccess = item.ShipSuccess;
                obj.Status = item.Status;
                listData.Add(obj);
            });
            return new JsonResult()
            {
                Data = DataSourceLoader.Load(listData, options),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue
            };
        }
    }
}