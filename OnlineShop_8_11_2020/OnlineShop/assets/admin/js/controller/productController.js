var product = {
    init: function () {
        product.registerEvents();
    },
    //vì ở dưới cùng boby nên ko cần document ready
    registerEvents: function () {
        //nút kích hoạt/khóa
        $('.btnStatus').off('click').on('click', function (e) {
            //preventDetault tránh link vào href="#"
            e.preventDefault();
            var btn = $(this);
            var id = btn.data('id');
            $.ajax({
                url: "/Admin/Product/ChangeStatus",
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
        //nút Quản lý ảnh
        $('.btnMoreImages').off('click').on('click', function (e) {
            e.preventDefault();
            //hiện modal
            $('#MoreImagesModal').modal('show');
            //productID
            $('#hidProductID').val($(this).data('id'));
            //hiện ảnh
            product.loadImages();
        });
        //pick images từ btnAddImages
        $('#btnAddImages').off('click').on('click', function (e) {
            e.preventDefault();
            var finder = new CKFinder();
            finder.selectActionFunction = function (url) {
                //hiện list images
                $('.imageList').append('<div style="float:left"><img src="' + url + '" height="100" /><a class="btn-delImage1"><i class="fa fa-times"></i></a><div>');
                //bỏ 1 image
                $('.btn-delImage1').off('click').on('click', function (e) {
                    e.preventDefault();
                    $(this).parent().remove();
                });
            };
            finder.popup();
        });
        //nút Lưu
        $('#btnSaveImages').off('click').on('click', function () {
            var images = [];
            //productID
            var id = $('#hidProductID').val();
            //lấy tất cả thuộc tính src trong class .imageList nhét vào mảng images
            $.each($('.imageList img'), function (i, item) {
                images.push($(item).prop('src'));
            })
            //Truyền id+images lên cho Admin/ProductController/SaveImages
            $.ajax({
                url: '/Admin/Product/SaveImages',
                type: 'POST',
                data: {
                    id: id,
                    //chuyển sang string
                    images: JSON.stringify(images)
                },
                dataType: 'json',
                //ẩn modal
                success: function (response) {
                    if (response.status == true) {
                        $('#MoreImagesModal').modal('hide');
                        //empty list ảnh
                        $('.imageList').html('');
                        //alert('Lưu thành công');
                    }
                },
            });
        });
    },

    //load ảnh
    loadImages: function () {
        $.ajax({
            url: '/Admin/Product/LoadImages',
            type: 'GET',
            data: {
                //productID
                id: $('#hidProductID').val()
            },
            dataType: 'json',
            success: function (response) {
                var data = response.data;
                var html = '';
                $.each(data, function (i, item) {
                    html += '<div style="float:left"><img src="' + item + '" height="100" /><a class="btn-delImage2"><i class="fa fa-times"></i></a></div>'
                });
                //hiện ảnh lên class .imageList
                $('.imageList').html(html);
                //nút x
                $('.btn-delImage2').off('click').on('click', function (e) {
                    e.preventDefault();
                    $(this).parent().remove();
                });
            },
        });
    }

}
product.init();
