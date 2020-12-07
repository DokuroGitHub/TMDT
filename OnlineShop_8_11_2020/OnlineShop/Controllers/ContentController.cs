using Model.Dao;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model.ViewModel;

namespace OnlineShop.Controllers
{
    public class ContentController : BaseController
    {
        // GET: Content
        public ActionResult Index(int pageIndex=1)
        {
            var model = new ContentDao().ListAllPaging(pageIndex, 10);

            return View(model);
        }

        [ChildActionOnly]
        public PartialViewResult Category()
        {
            var model = new CategoryDao().ListAll();
            return PartialView(model);
        }

        public ActionResult Detail(long id)
        {
            var content = new ContentDao().GetByID(id);

            ViewBag.Tags = new ContentDao().ListTag(id);
            ViewBag.Category = new CategoryDao().ViewDetail(content.CategoryID.Value);
            ViewBag.RalatedContents = new ContentDao().ListRelatedContent(id, 4);

            return View(content);
        }

        public ActionResult Tag(string tagId, int page=1, int pageSize=20)
        {
            var tag = new ContentDao().GetTag(tagId);
            ViewBag.Tag = tag;
            int totalRecord = 0;
            var model = new ContentDao().ListAllByTag(tagId, ref totalRecord, page, pageSize);

            ViewBag.Total = totalRecord;
            ViewBag.Page = page;
            //maxPage cho nút ở class pagination
            int maxPage = 5;
            int totalPage = 0;
            totalPage = (int)Math.Ceiling((decimal)totalRecord / pageSize);
            ViewBag.TotalPage = totalPage;
            ViewBag.MaxPage = maxPage;
            ViewBag.First = 1;
            ViewBag.Last = totalPage;
            ViewBag.Next = page + 1;
            ViewBag.Prev = page - 1;

            return View(model);
        }

        //Liệt kê tất cả content với categoryID
        public ActionResult ContentWithCategoryID(long categoryID, int pageIndex, int pageSize = 2)
        {
            var category = new CategoryDao().ViewDetail(categoryID);
            ViewBag.Category = category;
            int totalRecord = 0;
            var model = new ContentDao().ListByCategoryId(categoryID, ref totalRecord, pageIndex, pageSize);

            ViewBag.Total = totalRecord;
            ViewBag.Page = pageIndex;
            //maxPage cho nút ở class pagination
            int maxPage = 5;
            int totalPage = 0;
            totalPage = (int)Math.Ceiling((decimal)totalRecord / pageSize);
            ViewBag.TotalPage = totalPage;
            ViewBag.MaxPage = maxPage;
            ViewBag.First = 1;
            ViewBag.Last = totalPage;
            ViewBag.Next = pageIndex + 1;
            ViewBag.Prev = pageIndex - 1;

            return View(model);
        }
    }
}