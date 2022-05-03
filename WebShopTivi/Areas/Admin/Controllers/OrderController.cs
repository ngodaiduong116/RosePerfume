using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebShopTivi.Models.DAO;
using WebShopTivi.Models.EF;

namespace WebShopTivi.Areas.Admin.Controllers
{
    public class OrderController : BaseController
    {
        ShopTiviDBModel db = new ShopTiviDBModel();
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
            
            //Delete in order detail and Add turn quantity of product
            OrderDetail orderDetail = db.OrderDetails.FirstOrDefault(x => x.OrderId == order.Id);
            Product product = db.Products.FirstOrDefault(x => x.Id == orderDetail.ProductId);
            
            if (product != null)
            {
                product.Quantity += orderDetail.Quantity;
            }
            db.Orders.Remove(order);
            db.SaveChanges();
            SetAlert("Hủy đơn thành công", "success");
            return Redirect("/Admin/Order");
        }


        public JsonResult ChangeStatus(int? id)
        {
            var result = new OrderDAO().ChangeStatus(id);
            return Json(new
            {
                status = result
            });
        }

        [HttpPost]
        public ActionResult ExportToExcel()
        {
            var ExcelData = (from od in db.OrderDetails
                             join o in db.Orders
                             on od.OrderId equals o.Id
                             join p in db.Products
                             on od.ProductId equals p.Id
                             select new 
                             {
                                 Name = o.ShipName,
                                 Email = o.ShipEmail,
                                 Phone = o.ShipMobile,
                                 ProductName = p.Name,
                                 CreatedOn = o.CreateDate.Value,
                                 Price = od.Price.Value,
                                 Quantity = od.Quantity.Value,
                                 Total = od.Price.Value * od.Quantity.Value
                             }).ToList();
            var gv = new GridView();
            gv.DataSource = ExcelData;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=DemoExcel.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return View();
        }
    }
}
