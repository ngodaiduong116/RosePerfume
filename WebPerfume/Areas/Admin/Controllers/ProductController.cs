using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using WebPerfume.Models.DAO;
using WebPerfume.Models.EF;

namespace WebPerfume.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        private RosePerfumeDBModel db = new RosePerfumeDBModel();

        // GET: Admin/Product
        public ActionResult Index(string searchString, int page = 1, int pagesize = 5)
        {
            var dao = new ProductDAO();
            var model = dao.ListAllPaging(searchString, page, pagesize);
            ViewBag.SearchString = searchString;
            ViewBag.ListAll = new ProductDAO().ListAll();
            return View(model);
        }

        // GET: Admin/Product/Details/5
        public ActionResult Details(int id)
        {
            var getObg = db.Products.Find(id);
            return View(getObg);
        }

        // GET: Admin/Product/Create
        public ActionResult Create()
        {
            SetViewBagCategory();
            return View();
        }

        // POST: Admin/Product/Create
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(Product product, HttpPostedFileBase imgfile)
        {
            var sanpham = new Product();
            string path = uploadimage(imgfile);

            sanpham.CategoryId = product.CategoryId;
            sanpham.Name = product.Name;
            if (path == "-1")
            {
                sanpham.Image = "";
            }
            else
            {
                sanpham.Image = path;
            }
            sanpham.ImportPrice = product.ImportPrice;
            sanpham.Price = product.Price;
            sanpham.PromotionPrice = product.PromotionPrice;
            sanpham.Description = product.Description;
            sanpham.DescriptionDetail = product.DescriptionDetail;

            sanpham.Volume = product.Volume;
            sanpham.ReleaseYear = product.ReleaseYear;
            sanpham.Style = product.Style;
            sanpham.Sex = product.Sex;
            sanpham.Origin = product.Origin;

            sanpham.TopHot = product.TopHot;
            sanpham.Featured = product.Featured;
            sanpham.Quantity = product.Quantity;
            sanpham.CreateDate = DateTime.Now;

            sanpham.Status = product.Status;
            sanpham.CreateBy = (string)Session["UserAdmin"];
            double percent = Convert.ToDouble(((product.Price - product.PromotionPrice) / product.Price) * 100);
            sanpham.PercentPromotion = Math.Round(percent, 0);
            db.Products.Add(sanpham);
            db.SaveChanges();

            SetViewBagCategory();
            SetAlert("Thêm mới thành công", "success");
            return RedirectToAction("Index");
        }

        // GET: Admin/Product/Edit/5
        public ActionResult Edit(int id)
        {
            SetViewBagCategory();

            var model = new ProductDAO().ViewDetail(id);
            return View(model);
        }

        // POST: Admin/Product/Edit/5
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Product product, HttpPostedFileBase imgfile)
        {
            var sanpham = db.Products.Find(product.Id);
            //string path = uploadimage(imgfile);
            sanpham.CategoryId = product.CategoryId;
            sanpham.Name = product.Name;
            sanpham.Image = product.Image;
            sanpham.ImportPrice = product.ImportPrice;
            sanpham.Price = product.Price;
            sanpham.PromotionPrice = product.PromotionPrice;
            sanpham.Description = product.Description;
            sanpham.DescriptionDetail = product.DescriptionDetail;
            sanpham.Volume = product.Volume;
            sanpham.ReleaseYear = product.ReleaseYear;
            sanpham.Style = product.Style;
            sanpham.Sex = product.Sex;
            sanpham.Origin = product.Origin;
            sanpham.TopHot = product.TopHot;
            sanpham.Featured = product.Featured;
            sanpham.Quantity = product.Quantity;

            sanpham.Status = product.Status;
            double percent = Convert.ToDouble(((product.Price - product.PromotionPrice) / product.Price) * 100);
            sanpham.PercentPromotion = Math.Round(percent, 0);

            db.SaveChanges();

            SetViewBagCategory();
            SetAlert("Cập nhật thành công", "success");
            return RedirectToAction("Index");
        }

        // GET: Admin/Product/Delete/5
        public ActionResult Delete(int id)
        {
            new ProductDAO().Delete(id);
            SetAlert("Xóa thành công", "success");
            return RedirectToAction("Index");
        }

        public void SetViewBagCategory(int? selectedID = null)
        {
            var dao = new CategoryDAO();
            ViewBag.CategoryId = new SelectList(dao.ListName(), "Id", "Name", selectedID);
        }

        public string uploadimage(HttpPostedFileBase file)
        {
            string path = "-1";
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    path = Path.Combine(Server.MapPath("/Content/uploadimg/product/"), Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    path = "/Content/uploadimg/product/" + Path.GetFileName(file.FileName);
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

        public ActionResult Import()
        {
            return View();
        }
    }
}