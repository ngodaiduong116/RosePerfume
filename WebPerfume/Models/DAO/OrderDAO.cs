using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebPerfume.Common;
using WebPerfume.Models.EF;

namespace WebPerfume.Models.DAO
{
    public class OrderDAO
    {
        private RosePerfumeDBModel db = new RosePerfumeDBModel();

        public int Insert(Order order)
        {
            db.Orders.Add(order);
            db.SaveChanges();
            return order.Id;
        }

        public List<Order> ListAllPaging(string searchString, int page, int pagesize)
        {
            List<Order> model = db.Orders.Where(n => n.Status == EnumStatus.New || n.Status == EnumStatus.Pendding).ToList();
            if (!string.IsNullOrEmpty(searchString))
            {
                return model = model.Where(n => n.ShipName.Contains(searchString)).ToList();
            }
            else
            {
                return model.OrderBy(n => n.Id).ToPagedList(page, pagesize).ToList();
            }
        }

        //public bool ChangeStatus(int? id)
        //{
        //    var order = db.Orders.Find(id);
        //    order.Status = !order.Status;
        //    db.SaveChanges();
        //    return order.Status;
        //}
    }
}