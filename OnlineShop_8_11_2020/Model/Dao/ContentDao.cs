using Common;
using Model.EF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
using System.Web;
using System.Data.Entity.Validation;
using System.Globalization;
using Model.ViewModel;

namespace Model.Dao
{
    public class ContentDao
    {
        OnlineShopDbContext db = null;
        public static string USER_SESSION = "USER_SESSION";
        public ContentDao()
        {
            db = new OnlineShopDbContext();
        }
        //done
        public List<ContentViewModel> ListAllContent(ref int totalRecord, int pageIndex, int pageSize)
        {
            totalRecord = db.Contents.Where(x => x.Status == true).Count();
            var model = (from a in db.Contents
                         join b in db.Categories on a.CategoryID equals b.ID
                         where a.Status == true
                         select new
                         {
                             ID = a.ID,
                             Name = a.Name,
                             MetaTitle = a.MetaTitle,
                             Description = a.Description,
                             Image = a.Image,
                             CategoryID = a.CategoryID,
                             CateName = b.Name,
                             CateMetaTitle = b.MetaTitle,
                             Detail = a.Detail,
                             CreatedDate = a.CreatedDate,
                             CreatedBy = a.CreatedBy,
                             ModifiedDate = a.ModifiedDate,
                             ModifiedBy = a.ModifiedBy,
                             MetaKeywords = a.MetaKeywords,
                             MetaDescriptions = a.MetaDescriptions,
                             ViewCount = a.ViewCount,
                             Tags = a.Tags,
                             Language = a.Language,
                         }).AsEnumerable().Select(x => new ContentViewModel()
                         {
                             ID = x.ID,
                             Name = x.Name,
                             MetaTitle = x.MetaTitle,
                             Description = x.Description,
                             Image = x.Image,
                             CategoryID = x.CategoryID,
                             CateName = x.CateName,
                             CateMetaTitle = x.CateMetaTitle,
                             Detail = x.Detail,
                             CreatedDate = x.CreatedDate,
                             CreatedBy = x.CreatedBy,
                             ModifiedDate = x.ModifiedDate,
                             ModifiedBy = x.ModifiedBy,
                             MetaKeywords = x.MetaKeywords,
                             MetaDescriptions = x.MetaDescriptions,
                             ViewCount = x.ViewCount,
                             Tags = x.Tags,
                             Language = x.Language,
                         });
            return model.OrderByDescending(x => x.CreatedDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }
        //done
        public List<ContentViewModel> ListByCategoryId(long categoryID, ref int totalRecord, int pageIndex, int pageSize)
        {
            totalRecord = db.Contents.Where(x => x.CategoryID == categoryID).Count();
            var model = (from a in db.Contents
                         join b in db.Categories on a.CategoryID equals b.ID
                         where a.Status == true && a.CategoryID == categoryID
                         select new
                         {
                             ID = a.ID,
                             Name = a.Name,
                             MetaTitle = a.MetaTitle,
                             Description = a.Description,
                             Image = a.Image,
                             CategoryID = a.CategoryID,
                             CateName = b.Name,
                             CateMetaTitle = b.MetaTitle,
                             Detail = a.Detail,
                             CreatedDate = a.CreatedDate,
                             CreatedBy = a.CreatedBy,
                             ModifiedDate = a.ModifiedDate,
                             ModifiedBy = a.ModifiedBy,
                             MetaKeywords = a.MetaKeywords,
                             MetaDescriptions = a.MetaDescriptions,
                             ViewCount = a.ViewCount,
                             Tags = a.Tags,
                             Language = a.Language,
                         }).AsEnumerable().Select(x => new ContentViewModel()
                         {
                             ID = x.ID,
                             Name = x.Name,
                             MetaTitle = x.MetaTitle,
                             Description = x.Description,
                             Image = x.Image,
                             CategoryID = x.CategoryID,
                             CateName = x.CateName,
                             CateMetaTitle = x.CateMetaTitle,
                             Detail = x.Detail,
                             CreatedDate = x.CreatedDate,
                             CreatedBy = x.CreatedBy,
                             ModifiedDate = x.ModifiedDate,
                             ModifiedBy = x.ModifiedBy,
                             MetaKeywords = x.MetaKeywords,
                             MetaDescriptions = x.MetaDescriptions,
                             ViewCount = x.ViewCount,
                             Tags = x.Tags,
                             Language = x.Language,
                         });
            return model.OrderByDescending(x => x.CreatedDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public IEnumerable<Content> ListAllPaging(string searchString, int page, int pageSize)
        {
            IQueryable<Content> model = db.Contents;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Name.Contains(searchString));
            }
            return model.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
        }
        /// <summary>
        /// List all content for client
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public IEnumerable<Content> ListAllPaging(int page, int pageSize)
        {
            IQueryable<Content> model = db.Contents;
            return model.Where(x=>x.Status==true).OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
        }

        public IEnumerable<Content> ListAllByTag(string tag, ref int totalRecord, int page, int pageSize)
        {
            totalRecord = db.ContentTags.Where(x => x.TagID == tag).Count();
            var model = (from a in db.Contents
                         join b in db.ContentTags
                         on a.ID equals b.ContentID
                         where b.TagID == tag
                         select new
                         {
                             ID = a.ID,
                             Name = a.Name,
                             MetaTitle = a.MetaTitle,
                             CategoryID=a.CategoryID,
                             Image = a.Image,
                             Description = a.Description,
                             CreatedDate=a.CreatedDate,
                             CreatedBy = a.CreatedBy,
                             Tags=a.Tags

                         }).AsEnumerable().Select(x => new Content()
                         {
                             ID = x.ID,
                             Name =x.Name,
                             MetaTitle=x.MetaTitle,
                             CategoryID = x.CategoryID,
                             Image =x.Image,
                             Description=x.Description,
                             CreatedDate = x.CreatedDate,
                             CreatedBy = x.CreatedBy,
                             Tags = x.Tags
                         });
            return model.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
        }

        public long Insert(Content entity)
        {
            db.Contents.Add(entity);
            db.SaveChanges();
            return entity.ID;
        }

        public Content GetByID(long id)
        {
            return db.Contents.Find(id);
        }

        public Tag GetTag(string id)
        {
            return db.Tags.Find(id);
        }

        public long Create(Content content)
        {
            //datetime for language
            var ddMMyyyy = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss tt");
            var MMddyyyy = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss tt");
            if (content.Language == "vi")
            {
                content.CreatedDate = DateTime.Parse(ddMMyyyy);
            }
            else if (content.Language == "en")
            {
                content.CreatedDate = DateTime.Parse(MMddyyyy);
            }
            else
                content.CreatedDate = null;
            //xử lý alias
            //metatile null && name ko null
            if (string.IsNullOrEmpty(content.MetaTitle) && !string.IsNullOrEmpty(content.Name))
            {
                content.MetaTitle = StringHelper.ToUnsignString(content.Name);
            }
            content.ViewCount = 0; 
            db.Contents.Add(content);
            db.SaveChanges();
            //tags ko null
            if (!string.IsNullOrEmpty(content.Tags))
            {
                string[] tags = content.Tags.Split(',');
                foreach(var tag in tags)
                {
                    var tagId = StringHelper.ToUnsignString(tag);
                    var existedTag = this.CheckTag(tagId);
                    //insert to tag table
                    if (!existedTag)
                    {
                        this.InsertTag(tagId,tag);
                    }
                    //insert to content tag
                    var existedContentTag = this.CheckContentTag(content.ID, tagId);
                    if (!existedContentTag)
                    {
                        this.InsertContentTag(content.ID, tagId);
                    }
                }
            }
            return content.ID;
        }

        public long Update(Content entity)
        {
            try
            {
                var content = db.Contents.Find(entity.ID);
                //xử lý alias
                if (string.IsNullOrEmpty(entity.MetaTitle) && !string.IsNullOrEmpty(content.Name))
                {
                    entity.MetaTitle = StringHelper.ToUnsignString(entity.Name);
                }
                content.Name = entity.Name;
                content.MetaTitle = entity.MetaTitle;
                content.Description = entity.Description;
                content.Image = entity.Image;
                content.CategoryID = entity.CategoryID;
                content.Detail = entity.Detail;
                content.Warranty = entity.Warranty;
                //ko update createddate và creadtedby
                content.CreatedDate = content.CreatedDate;
                content.CreatedBy = content.CreatedBy;
                //datetime for language
                var ddMMyyyy = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss tt");
                var MMddyyyy = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss tt");
                if (entity.Language == "vi")
                {
                    content.ModifiedDate = DateTime.Parse(ddMMyyyy);
                }
                else if (entity.Language == "en")
                {
                    content.ModifiedDate = DateTime.Parse(MMddyyyy);
                }
                else
                    entity.ModifiedDate = null;
                content.ModifiedBy = entity.ModifiedBy;
                content.MetaKeywords = entity.MetaKeywords;
                content.MetaDescriptions = entity.MetaDescriptions;
                content.Status = entity.Status;
                content.TopHot = entity.TopHot;
                //ko update viewcount
                content.ViewCount = content.ViewCount;
                content.Tags = entity.Tags;
                content.Language = entity.Language;
                db.SaveChanges();
                //tags ko null
                if (!string.IsNullOrEmpty(content.Tags))
                {
                    string[] tags = content.Tags.Split(',');
                    foreach (var tag in tags)
                    {
                        var tagId = StringHelper.ToUnsignString(tag);
                        var existedTag = this.CheckTag(tagId);
                        //insert to tag table
                        if (!existedTag)
                        {
                            this.InsertTag(tagId, tag);
                        }
                        //insert to content tag
                        var existedContentTag = this.CheckContentTag(content.ID, tagId);
                        if (!existedContentTag)
                        {
                            this.InsertContentTag(content.ID, tagId);
                        }
                    }
                }
                return content.ID;
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

        public bool Delete(long id)
        {
            try
            {
                var content = db.Contents.Find(id);
                //xử lý tag
                if (!string.IsNullOrEmpty(content.Tags))
                    this.RemoveAllContentTag(content.ID);
                db.Contents.Remove(content);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void RemoveAllContentTag(long contentId)
        {
            db.ContentTags.RemoveRange(db.ContentTags.Where(x=>x.ContentID==contentId));
            db.SaveChanges();
        }

        public void InsertTag(string id, string name)
        {
            var tag = new Tag();
            tag.ID = id;
            tag.Name = name;
            db.Tags.Add(tag);
            db.SaveChanges();
        }

        public void InsertContentTag(long contentId,string tagId)
        {
            var contentTag = new ContentTag();
            contentTag.ContentID = contentId;
            contentTag.TagID = tagId;
            db.ContentTags.Add(contentTag);
            db.SaveChanges();
        }

        public bool CheckTag(string id)
        {
            return db.Tags.Count(x => x.ID == id) > 0;
        }

        public bool CheckContentTag(long contentId, string tagId)
        {
            var model =  db.ContentTags.Count(x => x.ContentID == contentId && x.TagID == tagId) > 0;
            return model;
        }

        public bool ChangeStatus(long id)
        {
            var content = db.Contents.Find(id);
            content.Status = !content.Status;
            db.SaveChanges();
            return content.Status;
        }

        public List<Tag> ListTag(long contentId)
        {
            var model = (from a in db.Tags
                         join b in db.ContentTags
                         on a.ID equals b.TagID
                         where b.ContentID == contentId
                         select new
                         {
                             ID = b.TagID,
                             Name = a.Name
                         }).AsEnumerable().Select(x => new Tag
                         {
                             ID = x.ID,
                             Name = x.Name
                         });
            return model.ToList();
        }

        public List<Content> ListRelatedContent(long contentId, int top)
        {
            var content = db.Contents.Find(contentId);
            return db.Contents.Where(x => x.ID != contentId && x.Status == true && x.CategoryID == content.CategoryID).OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }

    }
}
