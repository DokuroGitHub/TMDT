using Model.Dao;
using Model.EF;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Configuration;
using Common;
using PayPal.Api;

namespace OnlineShop.Controllers
{
    public class CartController : BaseController
    {
        private PayPal.Api.Payment payment;

        // GET: Cart
        public ActionResult Index()
        {
            var cart = Session[Common.CommonConstants.CART_SESSION];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }
            var coupon = Session[Common.CommonConstants.COUPON_SESSION];
            var list1 = new List<Coupon>();
            if (coupon != null)
            {
                list1 = (List<Coupon>)coupon;
            }
            ViewBag.ListCoupon = list1;
            return View(list);
        }

        [HttpGet]
        public ActionResult Payment()
        {
            var cart = Session[Common.CommonConstants.CART_SESSION];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }
            var coupon = Session[Common.CommonConstants.COUPON_SESSION];
            var list1 = new List<Coupon>();
            if (coupon != null)
            {
                list1 = (List<Coupon>)coupon;
            }
            ViewBag.ListCoupon = list1;
            return View(list);
        }

        //<summary>Thanh toán trực tiếp bằng tiền mặt</summary>
        [HttpPost]
        public ActionResult SendOrder(string shipName, string mobile, string address, string email)
        {
            try
            {
                UserLogin user = (UserLogin)Session[Common.CommonConstants.USER_SESSION];
                var cart = (List<CartItem>)Session[Common.CommonConstants.CART_SESSION];
                var itemList = new List<CartItem>();
                if (cart != null)
                {
                    itemList = (List<CartItem>)cart;
                }
                var coupon = (List<Coupon>)Session[Common.CommonConstants.COUPON_SESSION];
                var couponList = new List<Coupon>();
                if (coupon != null)
                {
                    couponList = (List<Coupon>)coupon;
                }
                var productDao = new ProductDao();
                decimal originPrice = 0;
                foreach (var item in itemList)
                {
                    //check quantity+status của product
                    if(productDao.CheckProduct(item.Product.ID,item.Quantity)==false)
                    {
                        var alertMessage = "Số lượng sản phẩm ["  +item.Product.Name +  "] đã hết. Xin vui lòng chọn sản phẩm khác.";
                        SetAlert(alertMessage, "error");
                        return Redirect("/het-san-pham");
                    }
                    if (item.Product.PromotionPrice != null)
                        originPrice += (item.Product.PromotionPrice.GetValueOrDefault(0) * item.Quantity);
                    else
                        originPrice += (item.Product.Price.GetValueOrDefault(0) * item.Quantity);
                }
                decimal newPrice = originPrice;
                foreach (var item in couponList)
                {
                    if (item.ByPercentage == true)
                    {
                        newPrice = (newPrice * (1 - item.DiscountBy / 100));
                    }
                    else
                    {
                        newPrice = newPrice - item.DiscountBy;
                    }
                }
                if (newPrice < 0)
                    newPrice = 0;
                //Gửi Email
                //đọc file neworder.html //email mẫu
                string content = System.IO.File.ReadAllText(Server.MapPath("/assets/client/template/neworder.html"));
                //replace các key {{}}
                content = content.Replace("{{CustomerName}}", shipName);
                //tùy chọn: cần 2 phone+email hoặc ít nhất 1
                int infoCount = (mobile != "" ? 1 : 0) + (email != "" ? 1 : 0);
                if (infoCount > 0)
                {
                    content = content.Replace("{{Phone}}", mobile != "" ? mobile : "trống");
                    content = content.Replace("{{Email}}", email != "" ? email : "trống");
                }
                else
                {
                    return Redirect("/loi-dat-hang");
                }
                content = content.Replace("{{Address}}", address);
                decimal ship = 100000; // ship 100k
                content = content.Replace("{{Ship}}", ship.ToString("N0") + "đ");
                if (newPrice == originPrice)    //ko có coupon, ko có newPrice
                {
                    content = content.Replace("{{OriginPrice}}", "Giá: " + originPrice.ToString("N0")+"đ");
                    content = content.Replace("{{NewPrice}}", "");
                }
                else
                {
                    content = content.Replace("{{OriginPrice}}", "Giá gốc: "+originPrice.ToString("N0")+"đ");
                    content = content.Replace("{{NewPrice}}", "Giá mới: "+newPrice.ToString("N0")+"đ");
                }
                content = content.Replace("{{Total}}", (newPrice+ship).ToString("N0")+"đ");
                //đọc tham số từ key trong Web.config của project OnlineShop
                var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
                //gửi đi 2 nơi
                new MailHelper().SendMail(toEmail, "Đơn hàng mới từ OnlineShop đến người cung cấp sản phẩm", content);
                new MailHelper().SendMail(email, "Đơn hàng mới từ OnlineShop đến người đặt đơn hàng", content);

                //Gửi thành công // bắt đầu xử lý database
                //ghi order vào databasse
                var orderDao = new OrderDao();
                var order = new Model.EF.Order();
                order.CreatedDate = DateTime.Now;
                if (user != null)
                {
                    order.CustomerID = user.UserID;
                }
                order.ShipName = shipName;
                order.ShipMobile = mobile;
                order.ShipAddress = address;
                order.ShipEmail = email;
                order.Status = -1; //đang chờ shipper nhận
                var orderId = orderDao.Insert(order);
                //xử lý OrderDetail + Product
                var detailDao = new OrderDetailDao();

                foreach (var item in itemList)
                {
                    //Xử lý OrderDetail
                    var orderDetail = new OrderDetail();
                    orderDetail.ProductID = item.Product.ID;
                    orderDetail.OrderID = orderId;
                    orderDetail.Price = item.Product.Price;
                    orderDetail.Quantity = item.Quantity;
                    detailDao.Insert(orderDetail);
                    //Xử lý sản phẩm // cập nhật số lượng sản phẩm
                    productDao.UpdateQuantity(item.Product.ID, item.Quantity);
                }
                //xử lý coupon + CouponUser
                var couponDao = new CouponDao();
                var couponUserDao = new CouponUserDao();
                foreach (var item in couponList)
                {
                    //xử lý coupon
                    couponDao.UseCouponDiscountCode(item.Code);
                    //Xử lý CouponUser
                    var couponUser = new CouponUser();
                    couponUser.CouponID = item.ID;
                    couponUser.UserID = user.UserID;
                    couponUserDao.Insert(couponUser);
                }
                //gán cart+coupon null
                Session[Common.CommonConstants.CART_SESSION] = null;
                Session[Common.CommonConstants.COUPON_SESSION] = null;
            }
            catch (Exception ex)
            {
                //ghi log
                Logger.Log("Error" + ex.Message);
                return Redirect("/loi-dat-hang");
            }
            return Redirect("/hoan-thanh");
        }

        //Pay=paypal
        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            UserLogin user = (UserLogin)Session[Common.CommonConstants.USER_SESSION];
            var cart = (List<CartItem>)Session[Common.CommonConstants.CART_SESSION];
            var itemList = new List<CartItem>();
            if (cart != null)
            {
                itemList = (List<CartItem>)cart;
            }
            var coupon = (List<Coupon>)Session[Common.CommonConstants.COUPON_SESSION];
            var couponList = new List<Coupon>();
            if (cart != null)
            {
                couponList = (List<Coupon>)coupon;
            }
            var productDao = new ProductDao();
            //check quantity+status của product
            foreach (var item in cart)
            {
                if (productDao.CheckProduct(item.Product.ID, item.Quantity) == false)
                {
                    var alertMessage = "Số lượng sản phẩm [" + item.Product.Name + "] đã hết. Xin vui lòng chọn sản phẩm khác.";
                    SetAlert(alertMessage, "error");
                    return Redirect("/het-san-pham");
                }
            }
            //getting the apiContext
            APIContext apiContext = helper.PaypalConfiguration.GetAPIContext();
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal
                //Payer Id will be returned when payment proceeds or click to pay
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist
                    //it is returned by the create function call of the payment class
                    // Creating a payment
                    // baseURL is the url on which paypal sendsback the data.
                    // So we have provided URL of this controller only
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority +
                                "/Cart/PaymentWithPaypal?";
                    //here we are generating guid for storing the paymentID received in session
                    //which will be used in the payment execution.
                    //guid we are generating for storing the paymentID received in session
                    //after calling the create function and it is used in the payment execution
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url
                    //on which payer is redirected for paypal account payment
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    //get links returned from paypal in response to Create function call
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This section is executed when we have received all the payments parameters
                    // from the previous call to the function Create
                    // Executing a payment
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    //If executed payment failed then we will show payment failure message to user
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        Session.Remove(guid);
                        return View("FailureView");
                    }

                    //Tới đây coi như thành công
                    //ghi order vào databasse
                    var orderDetailDao = new Model.Dao.OrderDetailDao();
                    var orderCounponDao = new Model.Dao.OrderCouponDao();
                    var order = new Model.EF.Order();
                    order.CreatedDate = DateTime.Parse(executedPayment.create_time);
                    if (user != null)
                    {
                        order.CustomerID = user.UserID;
                    }
                    order.ShipName = executedPayment.payer.payer_info.shipping_address.recipient_name;
                    order.ShipMobile = executedPayment.payer.payer_info.shipping_address.phone;
                    order.ShipAddress = executedPayment.payer.payer_info.shipping_address.line1 + ", "
                        + executedPayment.payer.payer_info.shipping_address.city + ", "
                        + executedPayment.payer.payer_info.shipping_address.state + ", "
                        + executedPayment.payer.payer_info.shipping_address.country_code + ".";
                    order.ShipEmail = executedPayment.payer.payer_info.email;
                    order.Status = -1; //đang chờ shipper nhận
                    var orderID = new OrderDao().Insert(order);
                    decimal sumProduct = 0;
                    foreach (var item in itemList)
                    {
                        //Xử lý orderDetail
                        var orderDetail = new OrderDetail();
                        orderDetail.ProductID = item.Product.ID;
                        orderDetail.OrderID = orderID;
                        decimal? tempPrice = item.Product.PromotionPrice != null ? item.Product.PromotionPrice : item.Product.Price;
                        orderDetail.Price = tempPrice;
                        orderDetail.Quantity = item.Quantity;
                        orderDetailDao.Insert(orderDetail);
                        //Xử lý sản phẩm // cập nhật số lượng sản phẩm
                        productDao.UpdateQuantity(item.Product.ID, item.Quantity);
                        //
                        sumProduct += orderDetail.Price.GetValueOrDefault(0) * orderDetail.Quantity.GetValueOrDefault(0);
                    }
                    //xử lý coupon + CouponUser
                    var couponDao = new CouponDao();
                    var couponUserDao = new CouponUserDao();
                    if (couponList != null)
                    {
                        foreach (var item in couponList)
                        {
                            //xử lý coupon // giảm quantity của coupon đi 1
                            couponDao.UseCouponDiscountCode(item.Code);
                            //Xử lý CouponUser
                            var couponUser = new CouponUser();
                            couponUser.CouponID = item.ID;
                            couponUser.UserID = user.UserID;
                            couponUserDao.Insert(couponUser);
                            //Xử lý orderCoupon
                            var orderCoupon = new OrderCoupon();
                            orderCoupon.OrderID = orderID;
                            orderCoupon.CouponID = item.ID;
                            decimal tempAmount = 0;
                            if (item.ByPercentage == true)
                            {
                                tempAmount = sumProduct * item.DiscountBy / 100; //discount theo %
                            }
                            else
                            {
                                tempAmount = item.DiscountBy; //discount cố định
                            }
                            orderCoupon.DiscountAmount = tempAmount;
                            orderCounponDao.Insert(orderCoupon);
                        }
                    }
                    //gán cart+coupon null
                    Session[Common.CommonConstants.CART_SESSION] = null;
                    Session[Common.CommonConstants.COUPON_SESSION] = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error" + ex.Message);
                return View("FailureView");
            }
            //on successful payment, show success page to user.
            return View("SuccessView");
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            var cart = Session[Common.CommonConstants.CART_SESSION];
            var itemInCartList = new List<CartItem>();
            if (cart != null)
            {
                itemInCartList = (List<CartItem>)cart;
            }
            var couponList = Session[Common.CommonConstants.COUPON_SESSION];
            var itemInCoupontList = new List<Coupon>();
            if (couponList != null)
            {
                itemInCoupontList = (List<Coupon>)couponList;
            }
            //create itemlist and add item objects to it
            var itemList = new ItemList() { items = new List<Item>() };
            //Các giá trị bao gồm danh sách sản phẩm, thông tin đơn hàng
            //Sẽ được thay đổi bằng hành vi thao tác mua hàng trên website
            //Adding Item Details like name, currency, price etc
            decimal sumProduct = 0;
            var productDao = new ProductDao();
            foreach (var item in itemInCartList)
            {
                decimal? tempPrice = item.Product.PromotionPrice != null ? item.Product.PromotionPrice / 20000 : item.Product.Price / 20000;
                string temp = tempPrice.GetValueOrDefault(0).ToString("N2") ;
                itemList.items.Add(new Item()
                {
                    //Thông tin đơn hàng
                    name = item.Product.Name,
                    currency = "USD",
                    price = temp,
                    quantity = item.Quantity.ToString(),
                    sku = "sku"
                });
                sumProduct += decimal.Parse(temp)*item.Quantity;
            }
            foreach (var item in itemInCoupontList)
            {
                decimal discountAmount = 0;
                if (item.ByPercentage == true)
                {
                    discountAmount = (sumProduct * item.DiscountBy / 100);
                }
                else
                {
                    discountAmount = item.DiscountBy / 20000;
                }
                itemList.items.Add(new Item()
                {
                    //Thông tin đơn hàng
                    name = item.Name,
                    currency = "USD",
                    price = (-discountAmount).ToString("N2"), //tạo item với price = -discountPrice
                    quantity = "1",
                    sku = "sku"
                });
            }
            decimal subTotal = 0;
            foreach (var item in itemList.items)
            {
                subTotal += decimal.Parse(item.price) * decimal.Parse(item.quantity);
            }
            //Hình thức thanh toán qua paypal
            var payer = new Payer() { payment_method = "paypal" };
            // Configure Redirect Urls here with RedirectUrls object
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            //các thông tin trong đơn hàng
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = "0",
                shipping = "5",
                subtotal = subTotal.ToString("N2")
            };
            //Đơn vị tiền tệ và tổng đơn hàng cần thanh toán
            //Final amount with details
            decimal tempAmount = decimal.Parse(details.tax) + decimal.Parse(details.shipping) + decimal.Parse(details.subtotal);
            var amount = new Amount()
            {
                currency = "USD",
                total = tempAmount.ToString("N2"), // Total must be equal to sum of shipping, tax and subtotal.
                details = details
            };
            var transactionList = new List<Transaction>();
            //Tất cả thông tin thanh toán cần đưa vào transaction
            // Adding description about the transaction
            transactionList.Add(new Transaction()
            {
                description = "Hóa đơn ngày: " + (DateTime.Now).ToString("dd/MM/yyyy"),
                invoice_number = "Hóa đơn lúc: " + (DateTime.Now).ToString("dd/MM/yyyy hh:mm:ss tt"), //Generate an Invoice No
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext
            var gido = this.payment.Create(apiContext);
            return gido;
        }
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            this.payment = new Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }

        //SendOrder Success
        public ActionResult Success()
        {
            return View();
        }
        //SendOrder error
        public ActionResult Error()
        {
            return View();
        }
        //Product Quantity run out
        public ActionResult ProductRunOut()
        {
            return View();
        }

        //thêm sản phẩm có productID vào giỏ CART_SESSION
        [HttpGet]
        public ActionResult AddItem(long productId, int quantity)
        {
            var product = new ProductDao().GetByID(productId);
            var cart = Session[Common.CommonConstants.CART_SESSION];
            //giỏ có sản phẩm
            if (cart != null)
            {
                var list = (List<CartItem>)cart;
                //giỏ có sản phẩm product
                if (list.Exists(x => x.Product.ID == productId))
                {
                    foreach (var item in list)
                    {
                        //Tăng Quantity cho item
                        if (item.Product.ID == productId)
                        {
                            item.Quantity += quantity;
                        }
                    }
                }
                //giỏ chưa có sản phẩm productId
                else
                {
                    //Tạo mới đối tượng cart item
                    var item = new CartItem();
                    item.Product = product;
                    item.Quantity = quantity;
                    //Đưa item vào list
                    list.Add(item);
                }
                //Gán list vào session
                Session[Common.CommonConstants.CART_SESSION] = list;
            }
            //giỏ hàng chưa có sản phẩm nào
            else
            {
                //Tạo mới list
                var list = new List<CartItem>();
                //Tạo mới đối tượng cart item
                var item = new CartItem();
                item.Product = product;
                item.Quantity = quantity;
                //Đưa item vào list
                list.Add(item);
                //Gán list vào session
                Session[Common.CommonConstants.CART_SESSION] = list;
            }
            return RedirectToAction("Index");
        }

        //thêm coupon vào COUPON_SESSION
        [HttpPost]
        public ActionResult AddCoupon(string code)
        {
            UserLogin user = (UserLogin)Session[Common.CommonConstants.USER_SESSION];
            //var cart = (List<CartItem>)Session[Common.CommonConstants.CART_SESSION];
            if (user == null)
            {
                SetAlert("Rất tiếc, bạn phải đăng nhập để thực hiện chức năng này.", "error");
                return View();
            }
            var couponUserDao = new CouponUserDao();
            var coupon = new CouponDao().GetByCode(code);
            if (coupon == null)
            {
                SetAlert("Mã giảm giá không chính xác. Xin nhập lại.", "error");
                return View();
            }
            else
            {
                if (user != null)
                {
                    //check mã đã xài chưa
                    if (couponUserDao.CheckCouponUser(coupon.ID, user.UserID) == true)
                    {
                        SetAlert("Bạn đã dùng mã giảm giá này trong đơn đặt hàng trước.", "error");
                        return View();
                    }
                }
                var couponList = Session[Common.CommonConstants.COUPON_SESSION];
                if (couponList != null)
                {
                    var list = (List<Coupon>)couponList;
                    if (list.Exists(x => x.Code == code))
                    {
                        //thông báo giỏ đã dùng mã này
                        SetAlert("Giỏ hàng của bạn đang sử dụng mã giảm giá này.", "error");
                    }
                    else
                    {
                        //Mã hợp lệ. Đưa coupon vào list
//                        list.Clear();   // chỉ có 1 coupon mỗi order // tùy chọn
                        list.Add(coupon);
                        SetAlert("Sử dụng mã giảm giá thành công.", "success");
                    }
                    //Gán list vào session
                    Session[Common.CommonConstants.COUPON_SESSION] = list;
                    return View();
                }
                //giỏ hàng chưa có
                else
                {
                    //Mã hợp lệ. Đưa coupon vào list
                    //Tạo mới list
                    var list = new List<Coupon>();
                    //Tạo mới đối tượng cart item
                    //Đưa item vào list
                    list.Add(coupon);
                    SetAlert("Sử dụng mã giảm giá thành công.", "success");
                    //Gán list vào session
                    Session[Common.CommonConstants.COUPON_SESSION] = list;
                    //chưa xài dc
                    return View();
                }
            }
            //return RedirectToAction("Index");
        }

        //ajax để update ngay trong giỏ hàng
        public JsonResult Update(string cartModel)
        {
            var jsonCart = new JavaScriptSerializer().Deserialize<List<CartItem>>(cartModel);
            var sessionCart = (List<CartItem>)Session[Common.CommonConstants.CART_SESSION];

            foreach (var item in sessionCart)
            {
                var jsonItem = jsonCart.SingleOrDefault(x => x.Product.ID == item.Product.ID);
                if (jsonItem != null)
                {
                    item.Quantity = jsonItem.Quantity;
                }
            }
            Session[Common.CommonConstants.CART_SESSION] = sessionCart;
            return Json(new
            {
                status = true
            });
        }

        //ajax để delete ngay trong giỏ hàng
        public JsonResult DeleteAll(string cartModel)
        {
            Session[Common.CommonConstants.CART_SESSION] = null;
            return Json(new
            {
                status = true
            });
        }

        //ajax để delete 1 sản phẩm ngay trong giỏ hàng
        public JsonResult Delete(long id)
        {
            var sessionCart = (List<CartItem>)Session[Common.CommonConstants.CART_SESSION];
            sessionCart.RemoveAll(x => x.Product.ID == id);
            Session[Common.CommonConstants.CART_SESSION] = sessionCart;
            return Json(new
            {
                status = true
            });
        }

        //ajax để delete 1 coupon ngay trong giỏ hàng
        public JsonResult DeleteCoupon(long id)
        {
            var sessionCoupon = (List<Coupon>)Session[Common.CommonConstants.COUPON_SESSION];
            sessionCoupon.RemoveAll(x => x.ID == id);
            Session[Common.CommonConstants.COUPON_SESSION] = sessionCoupon;
            return Json(new
            {
                status = true
            });
        }

        [HttpGet]
        public ActionResult MyShipList()
        {
            UserLogin user = (UserLogin)Session[Common.CommonConstants.USER_SESSION];
            if (user != null)
            {
                //List<ShipViewModel> //có listItem, listCoupon
                var model = new OrderDao().ListAllShip().Where(x => x.CustomerID == user.UserID).ToList();
                return View(model);
            }
            return View();
        }

        //ajax để cancel 1 order
        public JsonResult CancelOrder(long id)
        {
            new OrderDao().Delete(id);
            return Json(new
            {
                status = true
            });
        }

    }
}