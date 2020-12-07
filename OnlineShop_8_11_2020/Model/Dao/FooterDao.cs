using Common;
using Model.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class FooterDao
    {
        OnlineShopDbContext db = new OnlineShopDbContext();
        public FooterDao()
        {
            db = new OnlineShopDbContext();
        }

        public Footer GetFooter()
        {
            return db.Footers.FirstOrDefault(x => x.Status == true);
        }

        //admin xài
        public IEnumerable<Footer> ListAllPaging(string searchString, int page, int pageSize)
        {
            IQueryable<Footer> model = db.Footers;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Content.Contains(searchString));
            }
            return model.OrderBy(x => x.ID).ToPagedList(page, pageSize);
        }

        //
        public IEnumerable<Footer> ListAllPaging(int page, int pageSize)
        {
            IQueryable<Footer> model = db.Footers;
            return model.OrderBy(x => x.ID).ToPagedList(page, pageSize);
        }

        public List<string> ListName(string keyword)
        {
            return db.Products.Where(x => x.Name.Contains(keyword)).Select(x => x.Name).ToList();
        }

        //done
        public long Insert(Product entity)
        {
            db.Products.Add(entity);
            db.SaveChanges();
            return entity.ID;
        }
        //done
        public Footer GetByID(long id)
        {
            return db.Footers.Find(id);
        }
        //done
        public long Create(Footer footer)
        {
            db.Footers.Add(footer);
            db.SaveChanges();
            return footer.ID;
        }

        //done
        public long Update(Footer entity)
        {
            try
            {
                var footer = db.Footers.Find(entity.ID);
                footer.Content = entity.Content;
                footer.Status = entity.Status;
                db.SaveChanges();
                return footer.ID;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Delete(long id)
        {
            try
            {
                var product = db.Products.Find(id);
                db.Products.Remove(product);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //done
        public bool ChangeStatus(long id)
        {
            var footer = db.Footers.Find(id);
            if (footer.Status == null || footer.Status==false)
            {
                footer.Status = true;
                db.SaveChanges();
                return true;
            }
            else
            {
                footer.Status = false;
                db.SaveChanges();
                return false;
            }
        }

    }
}
