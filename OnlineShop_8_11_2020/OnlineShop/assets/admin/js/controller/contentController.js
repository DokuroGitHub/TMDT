var content = {
    init: function () {
        content.registerEvents();
    },
    //vì ở dưới cùng boby nên ko cần document ready
    registerEvents: function () {
        //nút kích hoạt/khóa
        $('.btn-active').off('click').on('click', function (e) {
            //preventDetault tránh link vào href="#"
            e.preventDefault();
            var btn = $(this);
            var id = btn.data('id');
            $.ajax({
                url: "/Admin/Content/ChangeStatus",
                data: { id: id },
                dataType: "json",
                type: "POST",
                success: function (response) {
                    console.log(response);
                    if (response.status == true) {
                        btn.text('Kích hoạt');
                    }
                    else {
                        btn.text('Khóa');
                    }
                }
            })
        });
    }

}
content.init();
