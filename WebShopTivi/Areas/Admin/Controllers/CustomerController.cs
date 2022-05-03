using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShopTivi.Models.DAO;
using WebShopTivi.Models.EF;

namespace WebShopTivi.Areas.Admin.Controllers
{
    public class CustomerController : BaseController
    {
        ShopTiviDBModel db = new ShopTiviDBModel();
        // GET: Admin/Customer
        public ActionResult Index(string searchString, int page = 1, int pagesize = 5)
        {
            var dao = new CustomerDAO();
            var model = dao.ListAllPaging(searchString, page, pagesize);
            ViewBag.SearchString = searchString;
            return View(model);
        }

        // GET: Admin/Customer/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/Customer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Customer/Create
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(Customer customer, HttpPostedFileBase imgfile)
        {
            string path = uploadimage(imgfile);

            var model = new Customer();
            if(db.Customers.Any(x=>x.Username == customer.Username))
            {
                ViewBag.ErrorLog = "Đã tồn tại tên tài khoản";
                return View();
            }
            model.Username = customer.Username;
            model.Password = customer.Password;
            if(path == "-1")
            {
                model.Image = "";
            }
            else
            {
                model.Image = path;
            }
            
            model.Status = customer.Status;
            model.Name = customer.Name;
            model.Mobile = customer.Mobile;
            model.Email = customer.Email;
            model.Address = customer.Address;
            db.Customers.Add(model);
            db.SaveChanges();
            SetAlert("Thêm mới thành công", "success");

            return RedirectToAction("Index");
        }

        // GET: Admin/Customer/Edit/5
        public ActionResult Edit(int id)
        {
            var model = db.Customers.Find(id);
            return View(model);
        }

        // POST: Admin/Customer/Edit/5
        [HttpPost]
        public ActionResult Edit(Customer customer, HttpPostedFileBase imgfile)
        {
            string path = uploadimage(imgfile);

            var model = db.Customers.Find(customer.Id);
            model.Username = customer.Username;
            model.Password = customer.Password;
            if(path == "-1")
            {
                model.Image = customer.Image;
            }
            else
            {
                model.Image = path;
            }
            
            model.Status = customer.Status;
            model.Name = customer.Name;
            model.Mobile = customer.Mobile;
            model.Email = customer.Email;
            model.Address = customer.Address;
            db.SaveChanges();
            SetAlert("Cập nhật thành công", "success");

            return RedirectToAction("Index");
        }

        // GET: Admin/Customer/Delete/5
        public ActionResult Delete(int id)
        {
            var model = db.Customers.Find(id);
            db.Customers.Remove(model);
            db.SaveChanges();
            SetAlert("Xóa thành công", "success");
            return RedirectToAction("Index");
        }

        // POST: Admin/Customer/Delete/5

        public string uploadimage(HttpPostedFileBase file)
        {
            string path = "-1";
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    path = Path.Combine(Server.MapPath("/Content/uploadimg/customer/"), Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    path = "/Content/uploadimg/customer/" + Path.GetFileName(file.FileName);
                }
                catch (Exception)
                {
                    path = "-1";
                }
            }
            else
            {
                Response.Write("Vui lòng chọn file ảnh");
                path = "-1";
            }
            return path;
        }

    }
}
