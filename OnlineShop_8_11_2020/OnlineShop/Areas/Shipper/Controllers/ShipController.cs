using Model.Dao;
using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model.ViewModel;
using OnlineShop.Common;

namespace OnlineShop.Areas.Shipper.Controllers
{
    //đang fix
    public class ShipController : BaseController
    {
        // GET: Shipper/Ship //cần paging
        
        public ActionResult Index(int pageIndex = 1)
        {
            //int totalRecord = 0;
            //int pageSize = 999;

            var model = new OrderDao().ListAllShip();

            //var model1 = new OrderDao().ListAllPaging(pageIndex, 10);
            return View(model);
        }

        [HttpPost]
        public ActionResult TakeShip(long id)
        {
            var dao = new OrderDao();
            var order = dao.GetByID(id);
            UserLogin user = (UserLogin)Session[CommonConstants.USER_SESSION];
            //-1 chưa có shipper nào nhận, 1 đang ship, 2 đã ship, -2 hủy ko nhận ship
            if (order == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (order.Status == -1)
            {
                dao.ChangeStatus(id, 1); //đang ship
                SetAlert("Đã duyệt: Nhận đơn ship thành công", "success");
                ModelState.AddModelError("", "Nhận đơn ship thành công.");

                //order add shipperID
                dao.AddShipperID(user.UserID, id);
            }
            //
            else if(order.Status==1)
            {
                //thông báo: Đơn hàng đã có người khác nhận ship.
                SetAlert("Lỗi: Đơn hàng đã có người khác nhận ship.", "error");
                ModelState.AddModelError("", "Đơn hàng đã có người khác nhận ship.");
            }
            else if (order.Status == 2)
            {
                //thông báo: Đơn hàng đã được ship xong.
                SetAlert("Lỗi: Đơn hàng đã được ship xong.", "error");
                ModelState.AddModelError("", "Đơn hàng đã được ship xong.");
            }
            else if (order.Status == -2)
            {
                //thông báo: Đơn hàng đã hủy, không cần ship nữa.
                SetAlert("Lỗi: Đơn hàng đã hủy, không cần ship nữa.", "error");
                ModelState.AddModelError("", "Đơn hàng đã hủy, không cần ship nữa.");
            }
            return RedirectToAction("Index", "Ship");
        }
        [HttpPost]
        public ActionResult DoneShip(long id)
        {
            var dao = new OrderDao();
            var order = dao.GetByID(id);
            UserLogin user = (UserLogin)Session[CommonConstants.USER_SESSION];
            //-1 chưa có shipper nào nhận, 1 đang ship, 2 đã ship, -2 hủy ko nhận ship
            if (order == null)
            {
                return RedirectToAction("MyShipList", "Ship");
            }
            if (order.Status == -1)
            {
                //thông báo: Đơn hàng chưa có shipper nào nhận.
                SetAlert("Lỗi: Đơn hàng chưa có shipper nào nhận.", "error");
                ModelState.AddModelError("", "Đơn hàng chưa có shipper nào nhận.");
            }
            //
            else if (order.Status == 1)
            {
                //thông báo: Đơn hàng đang được shipper này nhận ship.
                dao.ChangeStatus(id, 2); //đã ship
                SetAlert("Đã duyệt: Xác nhận đã giao đơn ship thành công", "success");
            }
            else if (order.Status == 2)
            {
                //thông báo: Đơn hàng đã được ship xong.
                SetAlert("Lỗi: Đơn hàng đã được ship xong.", "error");
                ModelState.AddModelError("", "Đơn hàng đã được ship xong.");
            }
            else if (order.Status == -2)
            {
                //thông báo: Đơn hàng đã hủy, không cần ship nữa.
                SetAlert("Lỗi: Đơn hàng đã hủy, không thể xác nhận đã giao.", "error");
                ModelState.AddModelError("", "Đơn hàng đã hủy, không thể xác nhận đã giao.");
            }
            return RedirectToAction("MyShipList", "Ship");
        }
        [HttpPost]
        public ActionResult CancelShip(long id)
        {
            var dao = new OrderDao();
            var order = dao.GetByID(id);
            UserLogin user = (UserLogin)Session[CommonConstants.USER_SESSION];
            //-1 chưa có shipper nào nhận, 1 đang ship, 2 đã ship, -2 hủy ko nhận ship
            if (order == null)
            {
                return RedirectToAction("MyShiplist", "Ship");
            }
            if (order.Status == -1)
            {
                //Lỗi: Đơn hàng bị hủy, không thể hủy nữa.
                SetAlert("Lỗi: Đơn hàng bị hủy, không thể hủy nữa.", "error");
                ModelState.AddModelError("", "Đơn hàng bị hủy, không thể hủy nữa.");
            }
            else if (order.Status == 1)
            {
                //thông báo: Xác nhận hủy đơn ship hàng thành công.
                dao.ChangeStatus(id, -2); //đã hủy
                SetAlert("Thành công: Xác nhận hủy đơn ship hàng thành công.", "success");
            }
            else if (order.Status == 2)
            {
                //thông báo: Đơn hàng đã được ship xong.
                SetAlert("Lỗi: Đơn hàng đã được ship xong, không thể hủy nữa.", "error");
                ModelState.AddModelError("", "Đơn hàng đã được ship xong, không thể hủy nữa.");
            }
            else if (order.Status == -2)
            {
                //thông báo: Đơn hàng đã hủy, không cần ship nữa.
                SetAlert("Lỗi: Đơn hàng đã hủy, không thể hủy nữa.", "error");
                ModelState.AddModelError("", "Đơn hàng đã hủy, không thể hủy nữa.");
            }
            return RedirectToAction("Index", "Ship");
        }


        public ActionResult Detail1Ship(long id)
        {
            var model = new OrderDao().View1Ship(id);
            return View(model);
        }

        [HttpGet]
        public ActionResult MyShipList()
        {
            UserLogin user = (UserLogin)Session[CommonConstants.USER_SESSION];
            if (user != null)
            {
                var model = new OrderDao().ListAllShip().Where(x => x.ShipperID == user.UserID).ToList();
                return View(model);
            }
            return View();
        }
    }
}