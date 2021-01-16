using Model.EF;
using Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class CategoryDao
    {
        OnlineShopDbContext db = null;

        public CategoryDao()
        {
            db = new OnlineShopDbContext();
        }

        public long Insert(Category category)
        {
            db.Categories.Add(category);
            db.SaveChanges();
            return category.ID;
        }

        public List<Category> ListAll()
        {
            return db.Categories.Where(x => x.Status == true).ToList();
        }

        public Category ViewDetail(long id)
        {
            return db.Categories.Find(id);
        }

        public Category TakeByAlias(string alias)
        {
            return db.Categories.Where(x=>x.MetaTitle.Contains(alias)).FirstOrDefault();
        }
    }
}
