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
        RosePerfumeDBModel db = new RosePerfumeDBModel();
        private string CartSession = "CartSession";
        // GET: Cart
        public async Task<ActionResult> Index()
        {

            var listResult = new List<Cart>();
            if ((string)Session["UserClientUsername"] != "")
            {
                var userCurrent = (string)Session["UserClientUsername"].ToString();
                var getCus = new CustomerDAO().getCustomer(userCurrent);
                var listProductInCart = db.Carts.Where(x => x.CustomerId == getCus.Id).ToList();
                listResult = listProductInCart;
                ViewBag.SoLuong = listProductInCart.Count;
            }
            else
            {
                var getCartOfSession = Session[CartSession];
                if (getCartOfSession != null)
                {
                    listResult = (List<Cart>)getCartOfSession;
                    ViewBag.SoLuong = ((List<Cart>)getCartOfSession).Count;
                }
                else
                {
                    ViewBag.SoLuong = 0;
                }
            }
            return View(listResult);
        }

        public async Task GetTemplate(object obj)
        {
            try
            {
                var domainName = new Uri($"{Request.Url.Scheme}://{Request.Url.Authority}/Cart/TemplateSendMail");
                byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
                var httpClient = new WebClient();
                httpClient.Encoding = Encoding.UTF8;
                var uploadPage = httpClient.UploadData(domainName, data);
                var str = httpClient.DownloadString(domainName);
                var result = Encoding.UTF8.GetString(uploadPage);

                //HttpClient client = new HttpClient();
                //var values = db.Accounts.FirstOrDefault(x => x.Id == 1);
                //var data = values.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => (string)prop.GetValue(values, null));
                //var content = new FormUrlEncodedContent(data);
                //var response = await client.PostAsync(domainName, content);
                //var responseString = await response.Content.ReadAsStringAsync();
            }
            catch(Exception ex)
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

        public ActionResult AddItem(int id, int quantity)
        {
            if ((string)Session["UserClientUsername"] != "")
            {
                var userCurrent = (string)Session["UserClientUsername"].ToString();
                var getCus = db.Customers.FirstOrDefault(x => x.Username == userCurrent);
                var getListProductInCart = db.Carts.Where(x => x.CustomerId == getCus.Id).ToList();
                if (getListProductInCart.Count != 0 && getListProductInCart.Exists(x => x.ProductId == id && x.CustomerId == getCus.Id) == true)
                {
                    var ext = getListProductInCart.Find(x => x.ProductId == id);
                    if(ext != null)
                    {
                        ext.Quantity += quantity;
                    }                   
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
                        ext.Quantity += quantity;
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
            return RedirectToAction("Index");
        }

        public JsonResult Update(string cartModel)
        {
            var jsonCart = new JavaScriptSerializer().Deserialize<List<CartItem>>(cartModel);
            var sessionCart = (List<CartItem>)Session[CartSession];
            foreach (var item in sessionCart)
            {
                var jsonItem = jsonCart.SingleOrDefault(n => n.Product.Id == item.Product.Id);
                if (jsonItem != null)
                {
                    item.quantity = jsonItem.quantity;
                    item.Total = jsonItem.quantity * item.Product.Price;
                }
            }
            Session[CartSession] = sessionCart;
            return Json(new
            {
                status = true
            });
        }
        
        public JsonResult DeleteAll()
        {
            Session[CartSession] = null;

            return Json(new
            {
                status = true
            });
        }

        public JsonResult Delete(int id)
        {
            var sessionCart = (List<CartItem>)Session[CartSession];
            sessionCart.RemoveAll(n => n.Product.Id == id);
            Session[CartSession] = sessionCart;
            return Json(new
            {
                status = true
            });
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
            if ((string)Session["UserClientUsername"] != "")
            {
                var username = (string)Session["UserClientUsername"];
                var customer = db.Customers.Where(x => x.Username == username).FirstOrDefault();
                order.ShipName = customer.Name;
                order.ShipAddress = customer.Address;
                order.ShipMobile = customer.Mobile;
                order.ShipEmail = customer.Email;
            }
            else
            {
                order.CreateDate = DateTime.Now;
                order.ShipAddress = address;
                order.ShipMobile = mobile;
                order.ShipName = shipName;
                order.ShipEmail = email;
            }           
            try
            {
                var id = new OrderDAO().Insert(order);
                var cart = (List<CartItem>)Session[CartSession];
                var detailDao = new OrderDetailDAO();
                //decimal total = 0;
                foreach (var item in cart)
                {
                    var orderDetail = new OrderDetail();
                    orderDetail.ProductId = item.Product.Id;
                    orderDetail.OrderId = id;
                    if(item.Product.PromotionPrice != null)
                    {
                        orderDetail.Price = item.Product.PromotionPrice;
                    }
                    else
                    {
                        orderDetail.Price = item.Product.Price;
                    }
                    orderDetail.Quantity = item.quantity;
                    detailDao.Insert(orderDetail);

                    //total += (item.SanPham.Gia.GetValueOrDefault(0) * item.quantity);

                    //Sub quantiy in Product table
                    var product = new ProductDAO();
                    product.setQuantity(orderDetail.ProductId, orderDetail.Quantity);
                }
                Session.Remove(CartSession); 
                SetAlert("Mua hàng thành công", "success");

                MailHelper obj = new MailHelper();
                //string dd = System.IO.File.ReadAllText(Server.MapPath("~/Common/Template/FormMail.html"));
                obj.SendMail("xoai2201@gmail.com", "Test", "Hello");
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
                var byee= memoryStream.ToArray();
                var str = Encoding.UTF8.GetString(byee);
                var order = JsonConvert.DeserializeObject<Order>(str);
                return PartialView(order);
            }
            
        }
    }
}