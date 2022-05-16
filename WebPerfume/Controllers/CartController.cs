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
            await GetTemplate( new {
                name = "Ngo Dai Duong",
                Age = 18
            });

            var cart = Session[CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
                ViewBag.SoLuong = list.Count;
            }
            else
            {
                ViewBag.SoLuong = 0;
            }
            return View(list);
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
            // kiểm tra là Khách Hàng có tài khoản không hay khách hàng vãng lai
            // Đã đăng kí tài khoản
            if ((string)Session["UserClientUsername"] != "")
            {
                var getCus = db.Customers.FirstOrDefault(x => x.Username == (string)Session["UserClientUsername"]);
                //Lấy danh sách sản phẩm có trong giỏ hàng hiện tại của khách hàng
                var getListProductInCart = db.Carts.Where(x => x.CustomerId == getCus.Id).ToList();
                if (getListProductInCart.Count == 0)
                {
                    var getProduct = new ProductDAO().ViewDetail(id);
                    var newItem = new Cart();
                    newItem.ProductId = id;
                    newItem.CustomerId = getCus.Id;
                    newItem.Product = getProduct;
                    newItem.Quantity = quantity;
                    newItem.Created = DateTime.Now;
                    if(getProduct.PromotionPrice != null || getProduct.PromotionPrice == 0)
                    {
                        newItem.Total = quantity * getProduct.Price;
                    }
                    else
                    {
                        newItem.Total = quantity * getProduct.PromotionPrice;
                    }
                    db.Carts.Add(newItem);
                    db.SaveChanges();
                }
                else
                {
                    var checkProduct = db.Carts.FirstOrDefault(x => x.ProductId.ToString().Contains(id.ToString()) && x.CustomerId == getCus.Id);
                    if (checkProduct != null)
                    {
                        var getProOfCart = db.Carts.FirstOrDefault(x => x.ProductId == id && x.CustomerId == getCus.Id);

                        getProOfCart.Quantity += quantity;
                        db.SaveChanges();
                    }
                    else
                    {
                        SetAlert("Sản phẩm đã ngưng bán", "warning");
                    }
                }
            }
            // Không có tài khoản Shop
            else
            {
                var productOfCart = Session[CartSession];
                if (productOfCart != null)
                {
                    var listProductByCart = (List<Cart>)productOfCart;
                    if (listProductByCart.Exists(e => e.ProductId == id))
                    {
                        foreach (var item in listProductByCart)
                        {
                            if (item.ProductId == id)
                            {
                                item.Quantity += quantity;
                                item.Created = DateTime.Now;
                            }
                        }
                    }
                    else
                    {
                        var newItem = new Cart();
                        newItem.ProductId = id;
                        newItem.Product = new ProductDAO().ViewDetail(id);
                        newItem.Quantity = quantity;
                        newItem.Created = DateTime.Now;
                    }
                }
            }




            var sp = new ProductDAO().ViewDetail(id);
            var cart = Session[CartSession];
            if (cart != null)
            {                
                var list = (List<CartItem>)cart;
                if (list.Exists(n => n.Product.Id == id))
                {
                    foreach (var item in list)
                    {
                        if (item.Product.Id == id)
                        {
                            item.quantity += quantity;
                        }
                    }
                }
                else
                {
                    var item = new CartItem();
                    item.Product = sp;
                    item.quantity = quantity;
                    if (item.Product.PromotionPrice != null)
                    {
                        item.Total = quantity * item.Product.PromotionPrice;
                    }
                    else
                    {
                        item.Total = quantity * item.Product.Price;
                    }

                    list.Add(item);
                }
                Session[CartSession] = list;
            }
            else
            {
                var item = new CartItem();
                item.Product = sp;
                item.quantity = quantity;
                if (item.Product.PromotionPrice != null)
                {
                    item.Total = quantity * item.Product.PromotionPrice;
                }
                else
                {
                    item.Total = quantity * item.Product.Price;
                }
                var list = new List<CartItem>();
                list.Add(item);
                Session[CartSession] = list;
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
            var cart = Session[CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
                //ViewBag.SoLuong = list.Count;
            }
            return View(list);
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