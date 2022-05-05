using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebPerfume.Models;
using WebPerfume.Models.DAO;
using WebPerfume.Models.EF;

namespace WebPerfume.Controllers
{
    public class CartController : BaseController
    {
        RosePerfumeDBModel db = new RosePerfumeDBModel();
        private string CartSession = "CartSession";
        // GET: Cart
        public ActionResult Index()
        {
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

        public ActionResult AddItem(int id, int quantity)
        {
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
            var order = new Order();
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
    }
}