using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using WebPerfume.Common;
using WebPerfume.Models;
using WebPerfume.Models.DAO;
using WebPerfume.Models.EF;
using Newtonsoft.Json;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace WebPerfume.Controllers
{
    public class CartController : BaseController
    {
        private RosePerfumeDBModel db = new RosePerfumeDBModel();
        private string CartSession = "CartSession";

        // GET: Cart
        public async Task<ActionResult> Index()
        {
            var obj = new SendMailModel();
            obj.Title = "Test Mail";
            obj.Body = "Html";
            obj.Content = "Thông báo khách hàng đặt hàng thành công";
            //await GetTemplate(obj);

            var listResult = new List<Cart>();
            if ((string)Session["UserClientUsername"] != "")
            {
                var userCurrent = (string)Session["UserClientUsername"].ToString();
                var getCus = db.Customers.FirstOrDefault(x => x.Username == userCurrent);
                var listProductInCart = db.Carts.Where(x => x.CustomerId == getCus.Id).ToList();
                if(listProductInCart.Count != 0)
                {
                    var listItemReMove = new List<Cart>();
                    listProductInCart.ForEach(item =>
                    {
                        if(item.Product.Quantity == 0)
                        {
                            listItemReMove.Add(item);
                        }
                        else
                        {
                            listResult.Add(item);
                        }
                    });
                    if(listItemReMove.Count != 0)
                    {
                        db.Carts.RemoveRange(listItemReMove);
                        db.SaveChanges();
                    }
                }
                else
                {
                    listResult = listProductInCart;
                }                
                ViewBag.SoLuong = listResult.Count;
            }
            else
            {
                var getCartOfSession = Session[CartSession];
                if (getCartOfSession != null)
                {
                    var listProductInCart = (List<Cart>)getCartOfSession;
                    //listResult = listProductInCart;
                    var listItemReMove = new List<Cart>();
                    listProductInCart.ForEach(item =>
                    {
                        if (item.Product.Quantity == 0)
                        {
                            listItemReMove.Add(item);
                        }
                        else
                        {
                            listResult.Add(item);
                        }
                    });
                    ViewBag.SoLuong = listResult.Count;
                }
                else
                {
                    ViewBag.SoLuong = 0;
                }
            }
            return View(listResult);
        }

        public async Task GetTemplate(SendMailModel obj)
        {
            try
            {
                var domainName = new Uri($"{Request.Url.Scheme}://{Request.Url.Authority}/Cart/TemplateSendMail");
                byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
                var httpClient = new WebClient();
                httpClient.Encoding = Encoding.UTF8;
                var uploadPage = httpClient.UploadData(domainName, data);
                //var str = httpClient.DownloadString(domainName);
                var result = Encoding.UTF8.GetString(uploadPage);

                //HttpClient client = new HttpClient();
                //var values = db.Accounts.FirstOrDefault(x => x.Id == 1);
                //var data = values.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (string)prop.GetValue(values, null));
                //var content = new FormUrlEncodedContent(data);
                //var response = await client.PostAsync(domainName, content);
                //var responseString = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
            }
        }

        public static string RenderPartialToString(string controlName, object viewData)
        {
            ViewPage viewPage = new ViewPage() { ViewContext = new ViewContext() };

            viewPage.ViewData = new ViewDataDictionary(viewData);
            viewPage.Controls.Add(viewPage.LoadControl(controlName));

            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                using (HtmlTextWriter tw = new HtmlTextWriter(sw))
                {
                    viewPage.RenderControl(tw);
                }
            }

            return sb.ToString();
        }

        [HttpPost]
        public ActionResult AddItems(int id, int quantity)
        {
            int quantityProductCurrent = (int)db.Products.FirstOrDefault(x => x.Id == id).Quantity;
            if(quantityProductCurrent == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "error");
            }
            else
            {
                if ((string)Session["UserClientUsername"] != "")
                {
                    var userCurrent = (string)Session["UserClientUsername"].ToString();
                    var getCus = db.Customers.FirstOrDefault(x => x.Username == userCurrent);
                    var getListProductInCart = db.Carts.Where(x => x.CustomerId == getCus.Id).ToList();
                    if (getListProductInCart.Count != 0 && getListProductInCart.Exists(x => x.ProductId == id && x.CustomerId == getCus.Id))
                    {
                        var ext = getListProductInCart.Find(x => x.ProductId == id);
                        ext.Quantity = (ext.Quantity + quantity) > quantityProductCurrent ? quantityProductCurrent : (ext.Quantity + quantity);
                    }
                    else
                    {
                        var getProduct = db.Products.FirstOrDefault(x => x.Id == id);
                        var newItem = new Cart();
                        newItem.ProductId = id;
                        newItem.CustomerId = getCus.Id;
                        newItem.Product = getProduct;
                        newItem.Quantity = quantity;
                        newItem.Created = DateTime.Now;

                        db.Carts.Add(newItem);
                    }
                    db.SaveChanges();
                }
                else
                {
                    var productOfCart = Session[CartSession];
                    if (productOfCart != null && ((List<Cart>)productOfCart).Exists(x => x.ProductId == id) == true)
                    {
                        var listProductByCart = (List<Cart>)productOfCart;
                        var ext = listProductByCart.Find(x => x.ProductId == id);
                        if (ext != null)
                        {
                            ext.Quantity = (ext.Quantity + quantity) > quantityProductCurrent ? quantityProductCurrent : (ext.Quantity + quantity);
                            ext.Created = DateTime.Now;
                        }
                        Session[CartSession] = listProductByCart;
                    }
                    else
                    {
                        var getProduct = db.Products.FirstOrDefault(x => x.Id == id);
                        var newItem = new Cart();
                        newItem.ProductId = id;
                        newItem.Product = getProduct;
                        newItem.Quantity = quantity;
                        newItem.Created = DateTime.Now;

                        if (productOfCart == null)
                        {
                            var listResult = new List<Cart>();
                            listResult.Add(newItem);
                            Session[CartSession] = listResult;
                        }
                        else
                        {
                            var listProduct = (List<Cart>)Session[CartSession];
                            listProduct.Add(newItem);
                            Session[CartSession] = listProduct;
                        }
                    }
                }
                return RedirectToAction("Index", "Cart");
            }
        }

        [HttpPost]
        public ActionResult AddItem(int id, int quantity)
        {
            int quantityProductCurrent = (int)db.Products.FirstOrDefault(x => x.Id == id).Quantity;
            if (quantityProductCurrent == 0)
            {
                return Json(false);
            }
            else
            {
                if ((string)Session["UserClientUsername"] != "")
                {
                    var userCurrent = (string)Session["UserClientUsername"].ToString();
                    var getCus = db.Customers.FirstOrDefault(x => x.Username == userCurrent);
                    var getListProductInCart = db.Carts.Where(x => x.CustomerId == getCus.Id).ToList();
                    if (getListProductInCart.Count != 0 && getListProductInCart.Exists(x => x.ProductId == id && x.CustomerId == getCus.Id))
                    {
                        var ext = getListProductInCart.Find(x => x.ProductId == id);
                        ext.Quantity = (ext.Quantity + quantity) > quantityProductCurrent ? quantityProductCurrent : (ext.Quantity + quantity);
                    }
                    else
                    {
                        var getProduct = db.Products.FirstOrDefault(x => x.Id == id);
                        var newItem = new Cart();
                        newItem.ProductId = id;
                        newItem.CustomerId = getCus.Id;
                        newItem.Product = getProduct;
                        newItem.Quantity = quantity;
                        newItem.Created = DateTime.Now;

                        db.Carts.Add(newItem);
                    }
                    db.SaveChanges();
                }
                else
                {
                    var productOfCart = Session[CartSession];
                    if (productOfCart != null && ((List<Cart>)productOfCart).Exists(x => x.ProductId == id) == true)
                    {
                        var listProductByCart = (List<Cart>)productOfCart;
                        var ext = listProductByCart.Find(x => x.ProductId == id);
                        if (ext != null)
                        {
                            ext.Quantity = (ext.Quantity + quantity) > quantityProductCurrent ? quantityProductCurrent : (ext.Quantity + quantity);
                            ext.Created = DateTime.Now;
                        }
                        Session[CartSession] = listProductByCart;
                    }
                    else
                    {
                        var getProduct = db.Products.FirstOrDefault(x => x.Id == id);
                        var newItem = new Cart();
                        newItem.ProductId = id;
                        newItem.Product = getProduct;
                        newItem.Quantity = quantity;
                        newItem.Created = DateTime.Now;

                        if (productOfCart == null)
                        {
                            var listResult = new List<Cart>();
                            listResult.Add(newItem);
                            Session[CartSession] = listResult;
                        }
                        else
                        {
                            var listProduct = (List<Cart>)Session[CartSession];
                            listProduct.Add(newItem);
                            Session[CartSession] = listProduct;
                        }
                    }
                }
                //return RedirectToAction("Index", "Cart");
                return Json(true);
            }
        }

        public ActionResult AddItemFromDetailProduct(int id, int quantity)
        {
            int quantityProductCurrent = (int)db.Products.FirstOrDefault(x => x.Id == id).Quantity;
            if ((string)Session["UserClientUsername"] != "")
            {
                var userCurrent = (string)Session["UserClientUsername"].ToString();
                var getCus = db.Customers.FirstOrDefault(x => x.Username == userCurrent);
                var getListProductInCart = db.Carts.Where(x => x.CustomerId == getCus.Id).ToList();
                if (getListProductInCart.Count != 0 && getListProductInCart.Exists(x => x.ProductId == id))
                {
                    var getProductInCart = getListProductInCart.Find(x => x.ProductId == id);
                    if (getProductInCart.Quantity == quantityProductCurrent && quantity == 1)
                    {
                    }
                    else
                    {
                        getProductInCart.Quantity = quantity;
                    }
                }
                else
                {
                    var newProduct = new Cart();
                    newProduct.CustomerId = getCus.Id;
                    newProduct.ProductId = id;
                    newProduct.Quantity = quantity;
                    newProduct.Created = DateTime.Now;
                    newProduct.Product = db.Products.FirstOrDefault(x => x.Id == id);
                    newProduct.Customer = getCus;
                    db.Carts.Add(newProduct);
                }
                db.SaveChanges();
            }
            else
            {
                var resultData = new List<Cart>();
                var productOfCart = Session[CartSession];
                if(productOfCart != null)
                {
                    var listProductOfCart = (List<Cart>)productOfCart;
                    if(listProductOfCart.Exists(x => x.ProductId == id))
                    {
                        var getObj = listProductOfCart.Find(x => x.ProductId == id);
                        if(getObj.Quantity == quantityProductCurrent && quantity == 1)
                        {

                        }
                        else
                        {
                            getObj.Quantity = quantity;
                        }                        
                    }
                    else
                    {
                        var newProduct = new Cart();
                        newProduct.ProductId = id;
                        newProduct.Quantity = quantity;
                        newProduct.Created = DateTime.Now;
                        newProduct.Product = db.Products.FirstOrDefault(x => x.Id == id);
                        listProductOfCart.Add(newProduct);

                    }
                    resultData = listProductOfCart;
                }
                else
                {
                    var newProduct = new Cart();
                    newProduct.ProductId = id;
                    newProduct.Quantity = quantity;
                    newProduct.Created = DateTime.Now;
                    newProduct.Product = db.Products.FirstOrDefault(x => x.Id == id);
                    resultData.Add(newProduct);
                }
                Session[CartSession] = resultData;



                //if (productOfCart != null && ((List<Cart>)productOfCart).Count != 0 && ((List<Cart>)productOfCart).Exists(x => x.ProductId == id))
                //{
                //    var ext = ((List<Cart>)productOfCart).Find(x => x.ProductId == id);
                //    if (ext.Quantity == quantityProductCurrent && quantity == 1)
                //    {

                //    }
                //    {
                //        ext.Quantity = quantity;
                //    }
                //}
                //else
                //{
                //    if (((List<Cart>)productOfCart).Count != 0)
                //    {
                //        var newProduct = new Cart();
                //        newProduct.ProductId = id;
                //        newProduct.Quantity = quantity;
                //        newProduct.Created = DateTime.Now;
                //        newProduct.Product = db.Products.FirstOrDefault(x => x.Id == id);
                //        var result = ((List<Cart>)productOfCart).Add(newProduct);
                //    }
                //}
                //Session[CartSession] = listProductByCart;
            }
            return RedirectToAction("Index");
        }

        public JsonResult Update(string cartModel)
        {
            int quantityBefore = 1;
            bool flag = true;
            try
            {
                var jsonCart = new JavaScriptSerializer().Deserialize<Cart>(cartModel);
                int quantityProductCurrent = (int)db.Products.FirstOrDefault(x => x.Id == jsonCart.Product.Id).Quantity;
                if ((string)Session["UserClientUsername"] != "")
                {
                    var userCurrent = (string)Session["UserClientUsername"].ToString();
                    var getCus = db.Customers.FirstOrDefault(x => x.Username == userCurrent);
                    var getProductInCart = db.Carts.FirstOrDefault(x => x.CustomerId == getCus.Id && x.ProductId == jsonCart.Product.Id);
                    if (jsonCart.Quantity <= 0)
                    {
                        db.Carts.Remove(getProductInCart);
                        flag = false;
                    }
                    else
                    {
                        if (jsonCart.Quantity <= quantityProductCurrent)
                        {
                            getProductInCart.Quantity = jsonCart.Quantity;
                        }
                        else
                        {
                            getProductInCart.Quantity = quantityProductCurrent;
                            quantityBefore = quantityProductCurrent;
                        }
                    }

                    db.SaveChanges();
                }
                else
                {
                    var sessionCart = (List<Cart>)Session[CartSession];
                    var getProduct = sessionCart.Find(x => x.ProductId == jsonCart.Product.Id);
                    if (jsonCart.Quantity <= 0)
                    {
                        sessionCart.Remove(getProduct);
                        flag = false;
                    }
                    else
                    {
                        if (jsonCart.Quantity <= quantityProductCurrent)
                        {
                            getProduct.Quantity = jsonCart.Quantity;
                        }
                        else
                        {
                            getProduct.Quantity = quantityProductCurrent;
                            quantityBefore = quantityProductCurrent;
                        }
                    }
                    Session[CartSession] = sessionCart;
                }

                if(quantityBefore == 1 && flag == false){
                    //Xóa sản phẩm khỏi giỏ hàng
                    return Json(new
                    {                        
                        status = false,
                        quantityBefore = 0
                    });
                }
                else
                {
                    if(quantityBefore == 1)
                    {
                        // thay đổi số lượng sản phẩm trong giới hạn cho phép, cập nhập lại giỏ hàng, gán lại giá trị
                        return Json(new
                        {
                            status = true,
                            quantityBefore = 1
                        });
                    }
                    else
                    {
                        // thay đổi lớn hơn só lượng cho phép,gán lại số lượng max
                        return Json(new
                        {
                            status = true,
                            quantityBefore = quantityBefore
                        });
                    }
                }
            }
            catch
            {
                //Lỗi xảy ra xóa sản phẩm khỏi giỏ hàng
                return Json(new
                {
                    status = false,
                    quantityBefore = 0
                });
            }
        }

        public JsonResult DeleteAll()
        {
            Session[CartSession] = null;

            return Json(new
            {
                status = true
            });
        }

        // có dùng
        public JsonResult Delete(int id)
        {
            try
            {
                if ((string)Session["UserClientUsername"] != "")
                {
                    var userCurrent = (string)Session["UserClientUsername"].ToString();
                    var getCus = db.Customers.FirstOrDefault(x => x.Username == userCurrent);
                    var getProOfCart = db.Carts.Where(x => x.CustomerId == getCus.Id && x.ProductId == id).ToList();
                    db.Carts.RemoveRange(getProOfCart);
                    db.SaveChanges();
                }
                else
                {
                    var sessionCart = (List<Cart>)Session[CartSession];
                    sessionCart.RemoveAll(n => n.ProductId == id);
                    Session[CartSession] = sessionCart;
                }
                return Json(new
                {
                    status = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false
                });
            }
        }

        [HttpGet]
        public ActionResult Payment()
        {
            var listResult = new List<Cart>();
            if ((string)Session["UserClientUsername"] != "")
            {
                var userCurrent = (string)Session["UserClientUsername"].ToString();
                var getCus = new CustomerDAO().getCustomer(userCurrent);
                var listProductInCart = db.Carts.Where(x => x.CustomerId == getCus.Id).ToList();
                listResult = listProductInCart;
            }
            else
            {
                var getCartOfSession = Session[CartSession];
                listResult = (List<Cart>)getCartOfSession;
            }
            return View(listResult);
        }

        [HttpPost]
        public ActionResult Payment(string shipName, string mobile, string address, string email)
        {
            Order order = new Order();
            var listProduct = new List<Cart>();
            if ((string)Session["UserClientUsername"] != "")
            {
                var username = (string)Session["UserClientUsername"];
                var customer = db.Customers.Where(x => x.Username == username).FirstOrDefault();
                order.CustomerId = customer.Id;
                order.ShipName = customer.Name;
                order.ShipMobile = customer.Mobile;
                order.ShipAddress = customer.Address;
                order.ShipEmail = customer.Email;
                order.CreateDate = DateTime.Now;

                listProduct = db.Carts.Where(x => x.CustomerId == customer.Id).ToList();
            }
            else
            {
                order.ShipName = shipName;
                order.ShipMobile = mobile;
                order.ShipAddress = address;
                order.ShipEmail = email;
                order.CreateDate = DateTime.Now;

                listProduct = (List<Cart>)Session[CartSession];
            }
            try
            {
                db.Orders.Add(order);
                db.SaveChanges();
                foreach (var item in listProduct)
                {
                    var newOrderDetails = new OrderDetail();
                    newOrderDetails.OrderId = order.Id;
                    newOrderDetails.ProductId = item.ProductId;
                    newOrderDetails.Quantity = item.Quantity;
                    newOrderDetails.Price = (item.Product.PromotionPrice != null && item.Product.PromotionPrice.Value != 0) ? item.Product.PromotionPrice : item.Product.Price;
                    newOrderDetails.TotalMoney = newOrderDetails.Quantity * newOrderDetails.Price;
                    db.OrderDetails.Add(newOrderDetails);
                }

                if ((string)Session["UserClientUsername"] != "")
                {
                    var username = (string)Session["UserClientUsername"];
                    var customer = db.Customers.Where(x => x.Username == username).FirstOrDefault();
                    var getListProductInCart = db.Carts.Where(x => x.CustomerId == customer.Id).ToList();
                    db.Carts.RemoveRange(getListProductInCart);
                }
                else
                {
                    Session.Remove(CartSession);
                }
                db.SaveChanges();
                //MailHelper obj = new MailHelper();
                //string dd = System.IO.File.ReadAllText(Server.MapPath("~/Common/Template/FormMail.html"));
                //obj.SendMail("xoai2201@gmail.com", "Test", "Hello");
                SetAlert("Mua hàng thành công", "success");
            }
            catch (Exception ex)
            {
                //ghi log
                return Redirect("/loi-thanh-toan");
            }
            return Redirect("/");
        }

        public ActionResult Success()
        {
            return View();
        }

        public ActionResult TemplateSendMail()
        {
            using (var memoryStream = new MemoryStream())
            {
                Request.InputStream.CopyTo(memoryStream);
                var byee = memoryStream.ToArray();
                var str = Encoding.UTF8.GetString(byee);
                var order = JsonConvert.DeserializeObject<SendMailModel>(str);
                return PartialView(order);
            }
        }

        //public bool checkQuantityBeforeAddItem(int idProduct, int quantityAdd, int idCustomer = 0)
        //{
        //    bool flag = true;
        //    var getQuantityProduct = db.Products.FirstOrDefault(x => x.Id == idProduct).Quantity;
        //    if (idCustomer != 0)
        //    {
        //        var getQuantityProductInCart = db.Carts.FirstOrDefault(x => x.CustomerId == idCustomer && x.ProductId == idProduct);
        //        if (getQuantityProductInCart != null)
        //        {
        //        }
        //    }
        //}
    }
}