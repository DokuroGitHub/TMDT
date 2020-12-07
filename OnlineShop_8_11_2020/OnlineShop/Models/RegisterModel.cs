using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineShop.Models
{
    public class RegisterModel
    {
        [Key]
        public long ID { set; get; }

        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Yêu cầu nhập tên đăng nhập.")]
        public string UserName { set; get; }

        [Display(Name = "Mật khẩu")]
        [StringLength(50,MinimumLength =6,ErrorMessage ="Độ dài mật khẩu từ 6 đến 20 ký tự.")]
        [Required(ErrorMessage ="Yêu cầu nhập mật khẩu.")]
        public string Password { set; get; }

        [Display(Name = "Xác nhật mật khẩu")]
        [Compare("Password", ErrorMessage = "Xác nhận mật khẩu không đúng.")]
        [Required(ErrorMessage = "Yêu cầu nhập xác nhận mật khẩu.")]
        public string ConfirmPassword { set; get; }


        [Display(Name = "Họ tên")]
        [Required(ErrorMessage = "Yêu cầu nhập họ tên.")]
        public string Name { set; get; }

        [Display(Name = "Địa chỉ")]
        public string Address { set; get; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Yêu cầu nhập email.")]
        public string Email { set; get; }

        [Display(Name = "Điện thoại")]
        public string Phone { set; get; }

        [Display(Name = "Tỉnh thành")]
        public int? ProvinceID { set; get; }

        [Display(Name = "Quận/Huyện")]
        public int? DistrictID { set; get; }


    }
}