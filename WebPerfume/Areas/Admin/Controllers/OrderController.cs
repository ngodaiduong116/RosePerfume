using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebPerfume.Common;
using WebPerfume.Models.DAO;
using WebPerfume.Models.EF;

namespace WebPerfume.Areas.Admin.Controllers
{
    public class OrderController : BaseController
    {
        private RosePerfumeDBModel db = new RosePerfumeDBModel();

        // GET: Admin/Order
        public ActionResult Index(string searchString, int page = 1, int pagesize = 5)
        {
            var dao = new OrderDAO();
            var model = dao.ListAllPaging(searchString, page, pagesize);
            ViewBag.SearchString = searchString;
            return View(model);
        }

        // GET: Admin/Order/Details/5
        public ActionResult Details(int id)
        {
            Order order = db.Orders.Find(id);
            return View(db.OrderDetails.Where(x => x.OrderId == order.Id).ToList());
        }

        // GET: Admin/Order/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Order/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Order/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Admin/Order/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Order/Delete/5
        public ActionResult Delete(int id)
        {
            Order order = db.Orders.Find(id);
            order.Status = EnumStatus.Cancel;

            //Delete in order detail and Add turn quantity of product
            List<OrderDetail> listOrderDetail = db.OrderDetails.Where(x => x.OrderId == order.Id).ToList();
            listOrderDetail.ForEach(item =>
            {
                var getProduct = db.Products.FirstOrDefault(x => x.Id == item.ProductId);
                getProduct.Quantity += item.Quantity;
            });
            db.SaveChanges();
            // thông báo cho khách hàng tại đây
            SetAlert("Hủy đơn thành công", "success");
            return Redirect("/Admin/Order");
        }

        [HttpGet]
        public ActionResult HandleNewOrder(int id)
        {
            try
            {
                var getOrder = db.Orders.FirstOrDefault(x => x.Id == id);
                getOrder.Status = EnumStatus.Pendding;
                db.SaveChanges();
            }
            catch
            {
            }
            return Redirect("/Admin/Order");
        }

        [HttpGet]
        public ActionResult HandleApprove(int id)
        {
            try
            {
                var getOrder = db.Orders.FirstOrDefault(x => x.Id == id);
                getOrder.Status = EnumStatus.Approved;
                db.SaveChanges();
            }
            catch
            {
            }
            return Redirect("/Admin/Order");
        }

        //[HttpPost]
        //public ActionResult ExportToExcel()
        //{
        //    var ExcelData = (from od in db.OrderDetails
        //                     join o in db.Orders
        //                     on od.OrderId equals o.Id
        //                     join p in db.Products
        //                     on od.ProductId equals p.Id
        //                     select new
        //                     {
        //                         Name = o.ShipName,
        //                         Email = o.ShipEmail,
        //                         Phone = o.ShipMobile,
        //                         ProductName = p.Name,
        //                         CreatedOn = o.CreateDate.Value,
        //                         Price = od.Price.Value,
        //                         Quantity = od.Quantity.Value,
        //                         Total = od.Price.Value * od.Quantity.Value
        //                     }).ToList();
        //    var gv = new GridView();
        //    gv.DataSource = ExcelData;
        //    gv.DataBind();
        //    Response.ClearContent();
        //    Response.Buffer = true;
        //    Response.AddHeader("content-disposition", "attachment; filename=DemoExcel.xls");
        //    Response.ContentType = "application/ms-excel";
        //    Response.Charset = "";
        //    StringWriter objStringWriter = new StringWriter();
        //    HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
        //    gv.RenderControl(objHtmlTextWriter);
        //    Response.Output.Write(objStringWriter.ToString());
        //    Response.Flush();
        //    Response.End();
        //    return View();
        //}
    }
}