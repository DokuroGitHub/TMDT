using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineShop.Areas.Shipper.Model
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Xin nhập tên tài khoảng")]
        public string UserName { set; get; }
        
        [Required(ErrorMessage = "Xin nhập mật khẩu")]
        public string Password { set; get; }

        public bool RememberMe { set; get; }
    }
}