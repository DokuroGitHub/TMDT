using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.EF;
using Model.ViewModel;
using PagedList;

namespace Model.Dao
{
    public class OrderDao
    {
        OnlineShopDbContext db = null;
        public static string USER_SESSION = "USER_SESSION";
        public OrderDao()
        {
            db = new OnlineShopDbContext();
        }

        public long Insert(Order order)
        {
            db.Orders.Add(order);
            db.SaveChanges();
            return order.ID;
        }

        //search ShipName / for paging
        public IEnumerable<Order> ListAllPaging(string searchString, int page, int pageSize)
        {
            IQueryable<Order> model = db.Orders;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.ShipName.Contains(searchString));
            }
            return model.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
        }
        public IEnumerable<Order> ListAllPaging(int page, int pageSize)
        {
            IQueryable<Order> model = db.Orders;
            return model.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
        }

        //done
        public Order GetByID(long id)
        {
            return db.Orders.Find(id);
        }

        //chưa cần dùng tới
        public bool Delete(long id)
        {
            try
            {
                var order = db.Orders.Find(id);
                db.Orders.Remove(order);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //đổi trạng thái order(chưa ship -1, đang ship 1, đã ship 2, hủy -2)
        public int? ChangeStatus(long id, int status)
        {
            var order = db.Orders.Find(id);
            order.Status = status;
            db.SaveChanges();
            return order.Status;
        }

        public bool AddShipperID(long? shipperID, long? orderID)
        {
            try
            {
                var order = db.Orders.Find(orderID);
                order.ShipperID = shipperID;
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //chưa dùng tới
        /// <summary>list top Orders by createdDate</summary>
        public List<Order> ListOrder(int top =20)
        {
            return db.Orders.OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }
        /// <summary>list Orders chưa có ship nào nhận</summary>
        public List<Order> ListOrderTru1(int top=20)
        {
            return db.Orders.Where(x=>x.Status==1).OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }
        /// <summary>list Orders chưa đang được ship</summary>
        public List<Order> ListOrder1(int top=20)
        {
            return db.Orders.Where(x => x.Status == 1).OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }
        /// <summary>list Orders chưa đã được ship</summary>
        public List<Order> ListOrder2(int top=20)
        {
            return db.Orders.Where(x => x.Status == 2).OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }
        /// <summary>list Orders bị hủy ship</summary>
        public List<Order> ListOrderTru2(int top=20)
        {
            return db.Orders.Where(x => x.Status == -2).OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }


        //trả về List<ShipViewModel> //done
        public List<ShipViewModel> ListAllShip()
        {
            var model = (from a in db.Orders
                         select new
                         {
                             OrderID = a.ID,
                             CustomerID = a.CustomerID,
                             ShipToName = a.ShipName,
                             ShipMobile = a.ShipMobile,
                             ShipAddress =a.ShipAddress,
                             ShipEmail = a.ShipEmail,
                             tempShipperID = a.ShipperID,
                             Status = a.Status,
                         }).AsEnumerable().Select(x => new ShipViewModel()
                         {
                             OrderID = x.OrderID,
                             CustomerID = x.CustomerID,
                             ShipToName = x.ShipToName,
                             ShipMobile = x.ShipMobile,
                             ShipAddress = x.ShipAddress,
                             ShipEmail = x.ShipEmail,
                             ShipItemList = new OrderDetailDao().ListByOrderId(x.OrderID).ToList(),
                             CouponList = new OrderCouponDao().ListByOrderId(x.OrderID).ToList(),
                             ShipperID = x.tempShipperID,
                             ShipperName = x.tempShipperID != null ? new UserDao().GetByID(x.tempShipperID).Name : null,
                             Status = x.Status,
                         });
            //var gido = model.OrderByDescending(x => x.OrderID).ToList();
            var gido = model.OrderByDescending(x => x.OrderID).ToList();
            return gido;
        }

        //trả vê 1 ShipViewModel để xem detail
        public ShipViewModel View1Ship(long orderID)
        {
            var model1 = new OrderDao().GetByID(orderID);
            var model2 = new ShipViewModel();
            model2.OrderID = model1.ID;
            model2.CustomerID = model1.CustomerID;
            model2.ShipToName = model1.ShipName;
            model2.ShipMobile = model1.ShipMobile;
            model2.ShipAddress = model1.ShipAddress;
            model2.ShipEmail = model1.ShipEmail;
            model2.Status = model1.Status;
            model2.ShipItemList = new OrderDetailDao().ListByOrderId(model1.ID).ToList();
            return model2;
        }

    }
}
