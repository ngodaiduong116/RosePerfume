using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPerfume.Models.DAO;
using WebPerfume.Models.EF;

namespace WebPerfume.Areas.Admin.Controllers
{
    public class CategoryController : BaseController
    {
        // GET: Admin/Category
        public ActionResult Index(string searchString, int page = 1, int pagesize = 5)
        {
            var dao = new CategoryDAO();
            var model = dao.ListAllPaging(searchString, page, pagesize);
            ViewBag.SearchString = searchString;
            return View(model);
        }

        // GET: Admin/Category/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/Category/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Category/Create
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(Category category, HttpPostedFileBase imgfile)
        {
            string path = uploadimage(imgfile);
            if (path.Equals("-1"))
            {

            }
            else
            {
                var dao = new CategoryDAO();
                int id = dao.Insert(category, path);
                if (id > 0)
                {
                    SetAlert("Thêm mới thành công", "success");
                    return RedirectToAction("Index", "Category");
                }
                else
                {
                    Response.Write("Thêm thất bại");
                }
            }
            return RedirectToAction("Index");

        }

        // GET: Admin/Category/Edit/5
        public ActionResult Edit(int id)
        {
            var category = new CategoryDAO().ViewDetail(id);
            return View(category);
        }

        // POST: Admin/Category/Edit/5
        [HttpPost]
        public ActionResult Edit(Category category, HttpPostedFileBase imgfile)
        {
            string path = uploadimage(imgfile);

            if (ModelState.IsValid)
            {
                var dao = new CategoryDAO();
                var result = dao.Update(category, path);
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

        // GET: Admin/Category/Delete/5
        public ActionResult Delete(int id)
        {
            new CategoryDAO().Delete(id);
            SetAlert("Xóa thành công", "success");
            return RedirectToAction("Index");
        }

        public string uploadimage(HttpPostedFileBase file)
        {
            string path = "-1";
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    path = Path.Combine(Server.MapPath("/Content/uploadimg/category/"), Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    path = "/Content/uploadimg/category/" + Path.GetFileName(file.FileName);
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
