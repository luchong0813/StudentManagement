using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.ViewModels;

namespace StudentManagement.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<IdentityUser> _usermanager;
        private SignInManager<IdentityUser> _signmanager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _usermanager = userManager;
            _signmanager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //将数据从模型中复制到IdentityUser
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await _usermanager.CreateAsync(user, model.Password);

                //如果注册成功，则使用登录服务登录
                //并重定向到HomeController的索引操作
                if (result.Succeeded)
                {
                    await _signmanager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                //如果有任何错误，则天机到ModelState对象中
                //将由验证摘要标记助手显示到视图上
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signmanager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await _signmanager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);


                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }

                }



                ModelState.AddModelError(string.Empty, "登录失败，请重试");
            }

            return View(model);
        }

        [AllowAnonymous]
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> IsEmailUse(string email)
        {
            var user = await _usermanager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"邮箱：{email}已经被注册了！");
            }
        }
    }
}