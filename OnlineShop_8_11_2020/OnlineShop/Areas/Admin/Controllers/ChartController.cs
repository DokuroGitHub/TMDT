using Model.Dao;
using Model.EF;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Areas.Admin.Controllers
{
    public class ChartController : BaseController
    {
        // GET: Admin/Chart
        public ActionResult Index()
        {
            var dao = new ThongKeDao();
            var year = DateTime.Now.Year;
            var model = dao.ListOrder1Year(year);
            ViewBag.Year = year;
            return View(model);
        }
        //done
        public ActionResult ThongKeThuNhapTrongThang()
        {
            var dao = new ThongKeDao();
            var month = DateTime.Now.Month;
            var year = DateTime.Now.Year;
            var model = dao.ListOrder1Month(month, year);
            ViewBag.Month = month;
            ViewBag.Year = year;
            return View(model);
        }
        //done
        public ActionResult ThongKeThuNhapLastNdays()
        {
            var dao = new ThongKeDao();
            var model = dao.ListOrderLastNdays(7);
            ViewBag.Ndays = 7;
            return View(model);
        }
        //done
        public ActionResult ThongKeTongSoSanPham()
        {
            var dao = new ThongKeDao();
            var model = dao.ListProductCategoryView();
            return View(model);
        }
        //done
        public ActionResult ThongKeSoSanPhamBanDuoc()
        {
            var dao = new ThongKeDao();
            var model = dao.ListProductCategoryView();
            return View(model);
        }
        //done
        public ActionResult ThongKeThuNhapTheoDanhMuc()
        {
            var dao = new ThongKeDao();
            var model = dao.ListProductCategoryView();
            return View(model);
        }
        //done
        public ActionResult ThongKeTongGiaTriTheoDanhMuc()
        {
            var dao = new ThongKeDao();
            var model = dao.ListProductCategoryView();
            return View(model);
        }
    }
}