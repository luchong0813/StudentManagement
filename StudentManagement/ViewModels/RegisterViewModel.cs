using Microsoft.AspNetCore.Mvc;
using StudentManagement.CustomerMiddlewares.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "邮箱地址")]
        [ValidEmailDomain(allowedDomain: "outlook.com", ErrorMessage = "邮箱的地址后缀必须是outlook.com")]
        [Remote(action: "IsEmailUse", controller: "Account")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "两次密码输入不一致，请检查后重新输入")]
        [Display(Name = "确认密码")]
        public string ConfirmPassword { get; set; }
    }
}
