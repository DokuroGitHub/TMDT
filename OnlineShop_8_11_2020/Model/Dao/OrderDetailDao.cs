using Model.EF;
using Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class OrderDetailDao
    {
        OnlineShopDbContext db = null;
        public OrderDetailDao()
        {
            db = new OnlineShopDbContext();
        }

        public bool Insert(OrderDetail detail)
        {
            try
            {
                db.OrderDetails.Add(detail);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }

        }

        //done
        public List<OrderDetail> GetByOrderID(long orderID)
        {
            return db.OrderDetails.Where(x => x.OrderID == orderID).ToList();
        }
        //done
        public List<OrderDetail> GetByProductID(long productID)
        {
            return db.OrderDetails.Where(x => x.ProductID == productID).ToList();
        }

        //chưa cần dùng tới
        public bool DeleteByOrderID(long orderID)
        {
            try
            {
                var orderDetail = db.OrderDetails.Find(orderID);
                db.OrderDetails.Remove(orderDetail);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool DeleteByProductID(long productID)
        {
            try
            {
                var orderDetail = db.OrderDetails.Find(productID);
                db.OrderDetails.Remove(orderDetail);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //với orderID cho trước, trả về list sản phẩm trong order đó
        public List<ShipItem> ListByOrderId(long orderID)
        {
            var model = (from a in db.OrderDetails
                         join b in db.Products on a.ProductID equals b.ID
                         where a.OrderID == orderID
                         select new
                         {
                             ProductID = a.ProductID,
                             Image = b.Image,
                             Name = b.Name,
                             MetaTitle = b.MetaTitle,
                             Price = a.Price,
                             Quantity = a.Quantity,
                             TotalPrice = a.Price*a.Quantity
                         }).AsEnumerable().Select(x => new ShipItem()
                         {
                             ProductID = x.ProductID,
                             Image = x.Image,
                             Name = x.Name,
                             MetaTitle =x.MetaTitle,
                             Price = x.Price.GetValueOrDefault(0),
                             Quantity = x.Quantity.GetValueOrDefault(0),
                             TotalPrice = x.TotalPrice.GetValueOrDefault(0)
                         });
            var gido = model.OrderBy(x => x.ProductID).ToList();
            return gido;
        }

    }
}
