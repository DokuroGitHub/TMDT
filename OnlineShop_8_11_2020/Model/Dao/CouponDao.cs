using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Model.EF;
using PagedList.Mvc;
using PagedList;
using System.Configuration;

namespace Model.Dao
{
    public class CouponDao
    {
        OnlineShopDbContext db = null;
        public CouponDao()
        {
            db = new OnlineShopDbContext();
        }
        //done
        public long Insert(Coupon entity)
        {
            db.Coupons.Add(entity);
            db.SaveChanges();
            return entity.ID;
        }

        //done
        public long Update(Coupon entity)
        {
            try
            {
                //lấy coupon ra để update
                var coupon = db.Coupons.Find(entity.ID);
                coupon.Name = entity.Name;
                coupon.Code = entity.Code;
                coupon.Quantity = entity.Quantity;
                coupon.ByPercentage = entity.ByPercentage;
                coupon.DiscountBy = entity.DiscountBy;
                coupon.Status = entity.Status;
                db.SaveChanges();
                return coupon.ID;
            }
            catch (Exception ex)
            {
                //logging
                Common.Logger.Log("Error" + ex.Message);
                return 0;
            }
        }

        public IEnumerable<User> ListAllPaging(string searchString, int page)
        {

            var pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"].ToString());
            IQueryable<User> model = db.Users;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.UserName.Contains(searchString) || x.Name.Contains(searchString));
            }
            return model.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
        }

        public Coupon GetByName(string name)
        {
            return db.Coupons.FirstOrDefault(x => x.Name == name);
        }

        public Coupon GetByID(long? id)
        {
            return db.Coupons.Find(id);
        }

        public Coupon GetByCode(string code)
        {
            return db.Coupons.FirstOrDefault(x => x.Code == code);
        }

        public bool Delete(int id)
        {

            try
            {
                var user = db.Users.Find(id);
                db.Users.Remove(user);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool ChangeStatus(long id)
        {
            var user = db.Users.Find(id);
            user.Status = !user.Status;
            db.SaveChanges();
            return user.Status;
        }

        //done
        public bool UseCouponDiscountCode(string code)
        {
            var coupon = db.Coupons.FirstOrDefault(x=>x.Code==code);
            if(coupon!=null)
            {
                if(coupon.Status==false)
                    return false; //coupon status false
                else
                {
                    if (coupon.Quantity > 0)
                    {
                        //coupon valid
                        coupon.Quantity--;
                        return true;
                    }
                    else
                    return false; //coupon quantity = 0
                }
            }
            else
            return false; // coupon not exist
        }


    }
}
