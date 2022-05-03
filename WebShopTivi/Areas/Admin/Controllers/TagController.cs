using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShopTivi.Models.DAO;
using WebShopTivi.Models.EF;

namespace WebShopTivi.Areas.Admin.Controllers
{
    public class TagController : BaseController
    {
        RosePerfumeDBModel db = new RosePerfumeDBModel();
        // GET: Admin/Tag
        public ActionResult Index(string searchString, int page = 1, int pagesize = 5)
        {
            var dao = new TagDAO();
            var model = dao.ListAllPaging(searchString, page, pagesize);
            ViewBag.SearchString = searchString;
            return View(model);
        }

        // GET: Admin/Tag/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/Tag/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Tag/Create
        [HttpPost]
        public ActionResult Create(Tag tag)
        {
            var model = new Tag();
            model.Name = tag.Name;
            model.Status = tag.Status;
            model.CreateBy = (string)Session["UserAdmin"];
            model.CreateDate = DateTime.Now;

            db.Tags.Add(model);
            db.SaveChanges();
            SetAlert("Thêm mới thành công", "success");

            return RedirectToAction("Index");
        }

        // GET: Admin/Tag/Edit/5
        public ActionResult Edit(int id)
        {
            var model = db.Tags.Find(id);
            return View(model);
        }

        // POST: Admin/Tag/Edit/5
        [HttpPost]
        public ActionResult Edit(Tag tag)
        {
            var model = db.Tags.Find(tag.Id);
            model.Name = tag.Name;
            model.Status = tag.Status;
            
            db.SaveChanges();
            SetAlert("Cập nhật thành công", "success");

            return RedirectToAction("Index");
        }

        // GET: Admin/Tag/Delete/5
        public ActionResult Delete(int id)
        {
            var model = db.Tags.Find(id);
            db.Tags.Remove(model);
            db.SaveChanges();

            SetAlert("Xóa thành công", "success");
            return RedirectToAction("Index");
        }

        // POST: Admin/Tag/Delete/5
        
    }
}
