using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model.Dao;
using System.Xml.Linq;

namespace OnlineShop.Controllers
{
    public class ProductController : BaseController
    {
        // GET: Product
        public ActionResult Index()
        {
            int pageIndex = 1;
            int totalRecord = 0;
            int pageSize = 20;
            //list productViewModel
            var model = new ProductDao().ListAllProduct(ref totalRecord, pageIndex, pageSize);
            //list productCategory để làm menu
            ViewBag.ListProductCategory = new ProductCateogryDao().ListAll();
            //4 sản phẩm nổi bật
            ViewBag.FeatureProducts = new ProductDao().ListFeatureProduct(4); ;
            return View(model);
        }

        [ChildActionOnly]
        public PartialViewResult ProductCategory()
        {
            var model = new ProductCateogryDao().ListAll();
            return PartialView(model);
        }

        public JsonResult ListName(string term)
        {
            var data = new ProductDao().ListName(term);
            return Json(new
            {
                data = data,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

        //Liệt kê tất cả product
        public ActionResult AllProduct(int pageIndex, int pageSize = 10)
        {
            int totalRecord = 0;
            var model = new ProductDao().ListAllProduct(ref totalRecord, pageIndex, pageSize);
            ViewBag.ListProductCategory = new ProductCateogryDao().ListAll();
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

        //Liệt kê tất cả product với categoryID
        public ActionResult ProductWithCategoryID(long categoryID, int pageIndex, int pageSize = 2)
        {
            var category = new ProductCateogryDao().ViewDetail(categoryID);
            ViewBag.ProductCategory = category;
            int totalRecord = 0;
            var model = new ProductDao().ListByCategoryId(categoryID, ref totalRecord, pageIndex, pageSize);

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

        public ActionResult Search(string keyword, int pageIndex, int pageSize = 2)
        {
            int totalRecord = 0;
            var model = new ProductDao().Search(keyword, ref totalRecord, pageIndex, pageSize);

            ViewBag.Total = totalRecord;
            ViewBag.Page = pageIndex;
            ViewBag.Keyword = keyword;
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

        //[OutputCache(CacheProfile = "Cache1DayForProduct")]
        public ActionResult Detail(long prodId)
        {
            var dao = new ProductDao();
            //lấy product
            var product = dao.GetByID(prodId);
            var listImage = dao.ListImages(prodId);
            ViewBag.ListImageSource = listImage;
            ViewBag.ProductCategory = new ProductCateogryDao().ViewDetail(product.CategoryID.Value);
            //4 sản phẩm liên quan
            ViewBag.RalatedProducts = dao.ListRelatedProduct(prodId, 4);
            return View(product);
        }

    }
}