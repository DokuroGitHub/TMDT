using Model.EF;
using Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class CouponUserDao
    {
        OnlineShopDbContext db = null;
        public CouponUserDao()
        {
            db = new OnlineShopDbContext();
        }

        public bool Insert(CouponUser entity)
        {
            try
            {
                db.CouponUsers.Add(entity);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        //done
        public bool CheckCouponUser(long couponID, long userID)
        {
            var model = db.CouponUsers.Where(x=>x.UserID == userID);
            if (model.Count()==0)
            {
                return false;
            }
            else
            {
                if (model.Where(x => x.CouponID == couponID).Count() == 0)
                    return false;
                else
                    return true; // user này đã dùng mã giảm giá này
            }
        }


        //chua fix



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

    }
}
