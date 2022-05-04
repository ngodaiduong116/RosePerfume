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
            return View();
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

        //[HttpPost, ValidateInput(false)]
        //public ActionResult Import(HttpPostedFileBase file)
        //{
        //    if (file.ContentType == "application/vnd.ms-excel" || file.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        //    {
        //        string filename = Path.GetFileName(Server.MapPath(file.FileName));
        //        var fileExt = Path.GetExtension(filename);
        //        ISheet sheet;
        //        if (fileExt == ".xls")
        //        {
        //            HSSFWorkbook hssfwb = new HSSFWorkbook(file.InputStream);
        //            sheet = hssfwb.GetSheetAt(0);
        //        }
        //        else
        //        {
        //            XSSFWorkbook hssfwb = new XSSFWorkbook(file.InputStream);
        //            sheet = hssfwb.GetSheetAt(0);
        //        }

        //        for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
        //        {
        //            var CategoryId = "";
        //            if (sheet.GetRow(i).GetCell(0).CellType == CellType.Numeric)
        //                CategoryId = sheet.GetRow(i).GetCell(0)?.NumericCellValue.ToString();
        //            else
        //                CategoryId = sheet.GetRow(i).GetCell(0)?.StringCellValue;
        //            var Name = "";
        //            if (sheet.GetRow(i).GetCell(1).CellType == CellType.Numeric)
        //                Name = sheet.GetRow(i).GetCell(1)?.NumericCellValue.ToString();
        //            else
        //                Name = sheet.GetRow(i).GetCell(1)?.StringCellValue;
        //            var Image = "";
        //            if (sheet.GetRow(i).GetCell(2).CellType == CellType.Numeric)
        //                Image = sheet.GetRow(i).GetCell(2)?.NumericCellValue.ToString();
        //            else
        //                Image = sheet.GetRow(i).GetCell(2)?.StringCellValue;
        //            var Image2 = "";
        //            if (sheet.GetRow(i).GetCell(3).CellType == CellType.Numeric)
        //                Image2 = sheet.GetRow(i).GetCell(3)?.NumericCellValue.ToString();
        //            else
        //                Image2 = sheet.GetRow(i).GetCell(3)?.StringCellValue;
        //            var ImportPrice = "";
        //            if (sheet.GetRow(i).GetCell(4).CellType == CellType.Numeric)
        //                ImportPrice = sheet.GetRow(i).GetCell(4)?.NumericCellValue.ToString();
        //            else
        //                ImportPrice = sheet.GetRow(i).GetCell(4)?.StringCellValue;
        //            var Price = "";
        //            if (sheet.GetRow(i).GetCell(5).CellType == CellType.Numeric)
        //                Price = sheet.GetRow(i).GetCell(5)?.NumericCellValue.ToString();
        //            else
        //                Price = sheet.GetRow(i).GetCell(5)?.StringCellValue;
        //            var PromotionPrice = "";
        //            if (sheet.GetRow(i).GetCell(6).CellType == CellType.Numeric)
        //                PromotionPrice = sheet.GetRow(i).GetCell(6)?.NumericCellValue.ToString();
        //            else
        //                PromotionPrice = sheet.GetRow(i).GetCell(6)?.StringCellValue;
        //            var Description = "";
        //            if (sheet.GetRow(i).GetCell(7).CellType == CellType.Numeric)
        //                Description = sheet.GetRow(i).GetCell(7)?.NumericCellValue.ToString();
        //            else
        //                Description = sheet.GetRow(i).GetCell(7)?.StringCellValue;
        //            var DescriptionDetail = "";
        //            if (sheet.GetRow(i).GetCell(8).CellType == CellType.Numeric)
        //                DescriptionDetail = sheet.GetRow(i).GetCell(8)?.NumericCellValue.ToString();
        //            else
        //                DescriptionDetail = sheet.GetRow(i).GetCell(8)?.StringCellValue;
        //            var ScreenSize = "";
        //            if (sheet.GetRow(i).GetCell(9).CellType == CellType.Numeric)
        //                ScreenSize = sheet.GetRow(i).GetCell(9)?.NumericCellValue.ToString();
        //            else
        //                ScreenSize = sheet.GetRow(i).GetCell(9)?.StringCellValue;
        //            var ScreenResolution = "";
        //            if (sheet.GetRow(i).GetCell(10).CellType == CellType.Numeric)
        //                ScreenResolution = sheet.GetRow(i).GetCell(10)?.NumericCellValue.ToString();
        //            else
        //                ScreenResolution = sheet.GetRow(i).GetCell(10)?.StringCellValue;

        //            var Quantity = "";
        //            if (sheet.GetRow(i).GetCell(11).CellType == CellType.Numeric)
        //                Quantity = sheet.GetRow(i).GetCell(11)?.NumericCellValue.ToString();
        //            else
        //                Quantity = sheet.GetRow(i).GetCell(11)?.StringCellValue;

        //            var product = new Product()
        //            {
        //                CategoryId = int.Parse(CategoryId),
        //                Name = Name,
        //                Image = Image,
        //                Image2 = Image2,
        //                ImportPrice = Convert.ToDecimal(ImportPrice),
        //                Price = Convert.ToDecimal(Price),
        //                PromotionPrice = Convert.ToDecimal(PromotionPrice),
        //                Description = Description,
        //                DescriptionDetail = DescriptionDetail,
        //                Volume = Volume;
        //                ReleaseYear = ReleaseYear;
        //                Style = Style;
        //                Sex = Sex;
        //                Origin = Origin;

        //            Quantity = int.Parse(Quantity),
        //                CreateDate = DateTime.Now

        //            };
        //            double percent = Convert.ToDouble(((Convert.ToDecimal(Price) - Convert.ToDecimal(PromotionPrice)) / Convert.ToDecimal(PromotionPrice)) * 100);
        //            product.PercentPromotion = Math.Round(percent, 0);

        //            db.Products.Add(product);
        //            db.SaveChanges();
        //        }
        //    }
        //    return RedirectToAction("Index");
        //}
    }
}