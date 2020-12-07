using Model.EF;
using Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class OrderCouponDao
    {
        OnlineShopDbContext db = null;
        public OrderCouponDao()
        {
            db = new OnlineShopDbContext();
        }
        
        public bool Insert(OrderCoupon entity)
        {
            try
            {
                db.OrderCoupons.Add(entity);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }

        }

        //done
        public List<OrderCoupon> GetByOrderID(long orderID)
        {
            return db.OrderCoupons.Where(x => x.OrderID == orderID).ToList();
        }
        //done
        public List<OrderCoupon> GetByCouponID(long couponID)
        {
            return db.OrderCoupons.Where(x => x.CouponID == couponID).ToList();
        }
        //done
        public List<CouponItem> ListByOrderId(long orderID)
        {
            var model = (from a in db.OrderCoupons
                         join b in db.Coupons on a.CouponID equals b.ID
                         where a.OrderID == orderID
                         select new
                         {
                             CouponID = b.ID,
                             Name = b.Name,
                             Code = b.Code,
                             ByPercentage = b.ByPercentage,
                             DiscountBy = b.DiscountBy,
                             DiscountAmount = a.DiscountAmount,
                         }).AsEnumerable().Select(x => new CouponItem()
                         {
                             CouponID = x.CouponID,
                             Name = x.Name,
                             Code = x.Code,
                             ByPercentage = x.ByPercentage,
                             DiscountBy = x.DiscountBy,
                             DiscountAmount = x.DiscountAmount.GetValueOrDefault(0),
                         });
            var gido = model.OrderBy(x => x.CouponID).ToList();
            return gido;
        }

        //chưa fix






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
        //chưa xài


    }
}
