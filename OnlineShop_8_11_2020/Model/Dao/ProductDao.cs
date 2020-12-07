using Common;
using Model.EF;
using Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
using System.Data.Entity.Validation;
using System.Xml.Linq;

namespace Model.Dao
{
    public class ProductDao
    {
        //done
        OnlineShopDbContext db = null;
        public static string USER_SESSION = "USER_SESSION";
        public ProductDao()
        {
            db = new OnlineShopDbContext();
        }
       
        public List<ProductViewModel> ListAllProduct(ref int totalRecord, int pageIndex, int pageSize)
        {
            totalRecord = db.Products.Where(x => x.Status==true).Count();
            var model = (from a in db.Products
                         join b in db.ProductCategories on a.CategoryID equals b.ID
                         where a.Status == true
                         select new
                         {
                             ID = a.ID,
                             Images = a.Image,
                             Name = a.Name,
                             Price = a.Price,
                             PromotionPrice = a.PromotionPrice,
                             Quantity = a.Quantity,
                             CateName = b.Name,
                             CateMetaTitle = b.MetaTitle,
                             MetaTitle = a.MetaTitle,
                             CreatedDate = a.CreatedDate
                         }).AsEnumerable().Select(x => new ProductViewModel()
                         {
                             ID = x.ID,
                             Images = x.Images,
                             Name = x.Name,
                             Price = x.Price,
                             PromotionPrice = x.PromotionPrice,
                             Quantity =x.Quantity,
                             CateName = x.CateName,
                             CateMetaTitle = x.CateMetaTitle,
                             MetaTitle = x.MetaTitle,
                             CreatedDate = x.CreatedDate
                         });
            return model.OrderByDescending(x => x.CreatedDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }
        /// <summary>
        /// Get list product by categoryId
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public List<ProductViewModel> ListByCategoryId(long categoryID, ref int totalRecord, int pageIndex, int pageSize)
        {
            totalRecord = db.Products.Where(x => x.CategoryID == categoryID).Count();
            var model = (from a in db.Products
                         join b in db.ProductCategories on a.CategoryID equals b.ID
                         where a.Status == true && a.CategoryID == categoryID
                         select new
                         {
                             ID = a.ID,
                             Images = a.Image,
                             Name = a.Name,
                             Price = a.Price,
                             PromotionPrice = a.PromotionPrice,
                             Quantity = a.Quantity,
                             CateName = b.Name,
                             CateMetaTitle = b.MetaTitle,
                             MetaTitle = a.MetaTitle,
                             CreatedDate = a.CreatedDate
                         }).AsEnumerable().Select(x => new ProductViewModel()
                         {
                             ID = x.ID,
                             Images = x.Images,
                             Name = x.Name,
                             Price = x.Price,
                             PromotionPrice = x.PromotionPrice,
                             Quantity = x.Quantity,
                             CateName = x.CateName,
                             CateMetaTitle = x.CateMetaTitle,
                             MetaTitle = x.MetaTitle,
                             CreatedDate = x.CreatedDate
                         });
            return model.OrderByDescending(x => x.CreatedDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        //admin xài
        public IEnumerable<Product> ListAllPaging(string searchString, int page, int pageSize)
        {
            IQueryable<Product> model = db.Products;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Name.Contains(searchString));
            }
            return model.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
        }
        public IEnumerable<Product> ListAllPaging(int page, int pageSize)
        {
            IQueryable<Product> model = db.Products;
            return model.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
        }

        public List<string> ListName(string keyword)
        {
            return db.Products.Where(x => x.Name.Contains(keyword)).Select(x => x.Name).ToList();
        }
        /// <summary>
        /// list new product
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public List<Product> ListNewProduct(int top)
        {
            return db.Products.Where(x => x.Status == true).OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }
        /// <summary>
        /// List feature product
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public List<Product> ListFeatureProduct(int top)
        {
            return db.Products.Where(x => x.Status == true && x.TopHot != null && x.TopHot > DateTime.Now).OrderByDescending(x => x.TopHot).Take(top).ToList();
        }
        /// <summary>
        /// Get list product by categoryId
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<ProductViewModel> Search(string keyword, ref int totalRecord, int pageIndex, int pageSize)
        {
            totalRecord = db.Products.Where(x => x.Name.Contains(keyword)).Count();
            //hàm Where làm đổi kiểu từ Entity sang LinQ
            var model = (from a in db.Products
                        where a.Status == true && a.Name.Contains(keyword)
                        select new
                        {
                            ID = a.ID,
                            Image = a.Image,
                            Name = a.Name,
                            Price = a.Price,
                            Quantity = a.Quantity,
                            PromotionPrice = a.PromotionPrice,
                            MetaTitle = a.MetaTitle,
                            CreatedDate = a.CreatedDate
                        }).AsEnumerable().Select(x=> new ProductViewModel()
                        {
                            ID = x.ID,
                            Images = x.Image,
                            Name = x.Name,
                            Price = x.Price,
                            PromotionPrice = x.PromotionPrice,
                            Quantity = x.Quantity,
                            MetaTitle = x.MetaTitle,
                            CreatedDate = x.CreatedDate
                        });
            return model.OrderByDescending(x => x.CreatedDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }
        //done
        public long Insert(Product entity)
        {
            db.Products.Add(entity);
            db.SaveChanges();
            return entity.ID;
        }
        //done
        public Product GetByID(long id)
        {
            return db.Products.Find(id);
        }
        //done
        public long Create(Product product)
        {
            var MMddyyyy = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss tt");
            var ddMMyyyy = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss tt");
            if (product.Language == "vi")
            {
                product.CreatedDate = DateTime.Parse(ddMMyyyy);
            }
            else if(product.Language == "en")
            {
                product.CreatedDate = DateTime.Parse(MMddyyyy);
            }
            else
                product.CreatedDate = null;
            //xử lý alias
            //metatile null && name ko null thì lấy UnsignStringName làm alias
            if (string.IsNullOrEmpty(product.MetaTitle) && !string.IsNullOrEmpty(product.Name))
            {
                product.MetaTitle = StringHelper.ToUnsignString(product.Name);
            }
            product.ViewCount = 0;
            db.Products.Add(product);
            db.SaveChanges();
            return product.ID;
        }
        //done
        public long Update(Product entity)
        {
            try
            {
                var product = db.Products.Find(entity.ID);
                //xử lý alias
                if (string.IsNullOrEmpty(entity.MetaTitle) && !string.IsNullOrEmpty(product.Name))
                {
                    entity.MetaTitle = StringHelper.ToUnsignString(entity.Name);
                }
                product.Name = entity.Name;
                product.Code = entity.Code;
                product.MetaTitle = entity.MetaTitle;
                product.Description = entity.Description;
                product.Image = entity.Image;
                product.Price = entity.Price;
                product.PromotionPrice = entity.PromotionPrice;
                product.Quantity = entity.Quantity;
                product.CategoryID = entity.CategoryID;
                product.Detail = entity.Detail;
                product.Warranty = entity.Warranty;
                //ko update createddate và creadtedby
                product.CreatedDate = product.CreatedDate;
                product.CreatedBy = product.CreatedBy;
                //datetime for language
                product.Language = entity.Language;
                var ddMMyyyy = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss tt");
                var MMddyyyy = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss tt");
                if (product.Language == "vi")
                {
                    product.ModifiedDate = DateTime.Parse(ddMMyyyy);
                }
                else if (product.Language == "en")
                {
                    product.ModifiedDate = DateTime.Parse(MMddyyyy);
                }
                else 
                    product.ModifiedDate = null;
                product.ModifiedBy = entity.ModifiedBy;
                product.MetaKeywords = entity.MetaKeywords;
                product.MetaDescriptions = entity.MetaDescriptions;
                product.Status = entity.Status;
                product.TopHot = entity.TopHot;
                //ko update viewcount
                product.ViewCount = product.ViewCount;
                db.SaveChanges();
                return product.ID;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }
        //done
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
            var product = db.Products.Find(id);
            product.Status = !product.Status;
            db.SaveChanges();
            return product.Status;
        }
        //check để mua
        public bool CheckProduct(long productID, long quantity)
        {
            var product = db.Products.Find(productID);
            return (product.Status)&&(product.Quantity>=quantity);
        }
        //done
        public bool UpdateQuantity(long productID, long quantity)
        {
            try
            {
                var product = db.Products.Find(productID);
                product.Quantity = product.Quantity - quantity;
                //update viewcount
                product.ViewCount++;
                db.SaveChanges();
                return true;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// list related product
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public List<Product> ListRelatedProduct(long productId, int top)
        {
            var product = db.Products.Find(productId);
            return db.Products.Where(x => x.ID != productId && x.Status == true && x.CategoryID == product.CategoryID).OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }
    
        public void UpdateImages(long productId, string images)
        {
            var product = db.Products.Find(productId);
            product.MoreImages = images;
            db.SaveChanges();
        }
        /// <summary>
        /// lấy List<string> ListImageSource
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<string> ListImages(long id)
        {
            var product = new ProductDao().GetByID(id);
            var images = product.MoreImages;
            XElement xImages;
            List<string> ListImagesReturn = new List<string>();
            if (images != null)
            {
                xImages = XElement.Parse(images);
                foreach (XElement element in xImages.Elements())
                {
                    ListImagesReturn.Add(element.Value);
                }
            }
            return ListImagesReturn;
        }
    }
}
