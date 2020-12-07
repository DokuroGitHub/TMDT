var cart = {
    init: function () {
        cart.regEvents();
    },
    regEvents: function () {
        //class "."
        //id "#"

        //Tiếp tục mua hàng
        $('#btnContinue').off('click').on('click', function () {
            window.location.href = "/";
        });
        //Update giỏ
        $('#btnUpdate').off('click').on('click', function () {
            var listProduct = $('.txtQuantity');
            var cartList = [];
            $.each(listProduct, function (i, item) {
                cartList.push({
                    Quantity: $(item).val(),
                    Product: {
                        ID: $(item).data('id')
                    }
                });
            });

            $.ajax({
                url: '/Cart/Update',
                data: { cartModel: JSON.stringify(cartList) },
                dataType: 'json',
                type: 'POST',
                success: function (res) {
                    if (res.status == true) {
                        window.location.href = "/gio-hang";
                    }
                }
            })

        });
        //DeleteAll
        $('#btnDeleteAll').off('click').on('click', function () {
            $.ajax({
                url: '/Cart/DeleteAll',
                dataType: 'json',
                type: 'POST',
                success: function (res) {
                    if (res.status == true) {
                        window.location.href = "/gio-hang";
                    }
                }
            })
        });
        //Delete 1 sản phẩm trong giỏ
        $('.btn-delete').off('click').on('click', function (e) {
            e.preventDefault();
            $.ajax({
                url: '/Cart/Delete',
                data: { id: $(this).data('id') },
                dataType: 'json',
                type: 'POST',
                success: function (res) {
                    if (res.status == true) {
                        window.location.href = "/gio-hang";
                    }
                }
            })
        });
        //Delete 1 coupon trong giỏ
        $('.btn-deleteCoupon').off('click').on('click', function (e) {
            e.preventDefault();
            $.ajax({
                url: '/Cart/DeleteCoupon',
                data: { id: $(this).data('id') },
                dataType: 'json',
                type: 'POST',
                success: function (res) {
                    if (res.status == true) {
                        window.location.href = "/gio-hang";
                    }
                }
            })
        });
        //Coupon discount code
        $('#btnCoupon').off('click').on('click', function () {
            $.ajax({
                url: '/Cart/AddCoupon',
                data: { code: $('.txtCoupon').val() },
                dataType: 'json',
                type: 'POST',
                complete: window.location.href = "/gio-hang"
            })
        });
        //Đơn đặt hàng của tôi
        $('#btnMyShipList').off('click').on('click', function () {
            window.location.href = "/don-dat-hang-cua-toi";
        });
        //Thanh toán trực tiếp bằng tiền mặt sau khi giao hàng
        $('#btnPayment').off('click').on('click', function () {
            window.location.href = "/thanh-toan";
        });
        //Thanh toán bằng Paypal trước khi giao hàng
        $('#btnPaymentWithPaypal').off('click').on('click', function () {
            window.location.href = "/thanh-toan-paypal";
        });
        //
        $('#btnTTTT').off('click').on('click', function () {
            window.location.href = "/thong-tin-thanh-toan";
        });
    }
}
cart.init();