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
    public class NewsController : BaseController
    {
        ShopTiviDBModel db = new ShopTiviDBModel();
        // GET: Admin/News
        public ActionResult Index(string searchString, int page = 1, int pagesize = 5)
        {
            var dao = new NewsDAO();
            var model = dao.ListAllPaging(searchString, page, pagesize);
            ViewBag.SearchString = searchString;
            return View(model);
        }

        // GET: Admin/News/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/News/Create
        public ActionResult Create()
        {
            SetViewBagTag();
            return View();
        }

        // POST: Admin/News/Create
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(News news, HttpPostedFileBase imgfile)
        {
            var x = new News();
            string path = uploadimage(imgfile);

            x.Name = news.Name;
            if(path == "-1")
            {
                x.Image = "";
            }
            else
            {
                x.Image = path;
            }
            
            x.Description = news.Description;
            x.DescriptionDetail = news.DescriptionDetail;
            x.CreateDate = DateTime.Now;
            x.CreateBy = "admin";
            x.TagId = news.TagId;

            db.News.Add(x);
            db.SaveChanges();


            SetAlert("Thêm mới thành công", "success");
            return RedirectToAction("Index");
        }

        // GET: Admin/News/Edit/5
        public ActionResult Edit(int id)
        {
            SetViewBagTag();
            var model = new NewsDAO().ViewDetail(id);
            return View(model);
        }

        // POST: Admin/News/Edit/5
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(News news, HttpPostedFileBase imgfile)
        {
            var x = db.News.Find(news.Id);
            var path = uploadimage(imgfile);

            if(path == "-1")
            {
                x.Image = news.Image;
            }
            else
            {
                x.Image = path;
            }
            x.Name = news.Name;
            x.Description = news.Description;
            x.DescriptionDetail = news.DescriptionDetail;
            x.CreateBy = "admin";
            x.CreateDate = DateTime.Now;
            x.TagId = news.TagId;

            db.SaveChanges();

            SetAlert("Cập nhật thành công", "success");
            return RedirectToAction("Index");
        }

        // GET: Admin/News/Delete/5
        public ActionResult Delete(int id)
        {
            new NewsDAO().Delete(id);
            SetAlert("Xóa thành công", "success");
            return RedirectToAction("Index");
        }

        public void SetViewBagTag(int? selectedID = null)
        {
            var dao = new TagDAO();
            ViewBag.TagId = new SelectList(dao.ListName(), "Id", "Name", selectedID);
        }

        public string uploadimage(HttpPostedFileBase file)
        {
            string path = "-1";
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    path = Path.Combine(Server.MapPath("/Content/uploadimg/news/"), Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    path = "/Content/uploadimg/news/" + Path.GetFileName(file.FileName);
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
