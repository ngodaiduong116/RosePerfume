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
    public class AccountController : BaseController
    {
        private RosePerfumeDBModel db = new RosePerfumeDBModel();

        // GET: Admin/Account
        public ActionResult Index(string searchString, int page = 1, int pagesize = 5)
        {
            var dao = new AccountDAO();
            var model = dao.ListAllPaging(searchString, page, pagesize);
            ViewBag.SearchString = searchString;
            return View(model);
        }

        // GET: Admin/Account/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/Account/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Account/Create
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(Account account, HttpPostedFileBase imgfile)
        {
            string path = uploadimage(imgfile);

            var tk = new Account();
            tk.Username = account.Username;
            tk.Password = account.Password;
            tk.Image = path;
            tk.Status = account.Status;
            tk.Fullname = account.Fullname;
            db.Accounts.Add(tk);
            db.SaveChanges();
            SetAlert("Thêm mới thành công", "success");

            return RedirectToAction("Index");
            //return RedirectToAction("Index");
        }

        // GET: Admin/Account/Edit/5
        public ActionResult Edit(int id)
        {
            var account = new AccountDAO().ViewDetail(id);
            return View(account);
        }

        // POST: Admin/Account/Edit/5
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Account account, HttpPostedFileBase imgfile)
        {
            string path = uploadimage(imgfile);

            if (ModelState.IsValid)
            {
                var dao = new AccountDAO();
                var result = dao.Update(account, path);
                if (result)
                {
                    SetAlert("Cập nhật thành công", "success");
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Sửa thất bại");
                }
            }

            return View("Index");
        }

        // GET: Admin/Account/Delete/5
        public ActionResult Delete(int id)
        {
            new AccountDAO().Delete(id);
            SetAlert("Xóa thành công", "success");
            return RedirectToAction("Index");
        }

        public JsonResult ChangeStatus(int? id)
        {
            var result = new AccountDAO().ChangeStatus(id);
            return Json(new
            {
                status = result
            });
        }

        public string uploadimage(HttpPostedFileBase file)
        {
            string path = "-1";
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    path = Path.Combine(Server.MapPath("/Content/uploadimg/account/"), Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    path = "/Content/uploadimg/account/" + Path.GetFileName(file.FileName);
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