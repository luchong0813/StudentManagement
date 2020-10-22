using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StudentManagement.Models;
using StudentManagement.ViewModels;

namespace StudentManagement.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> _usermanager;
        private SignInManager<ApplicationUser> _signmanager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger)
        {
            _usermanager = userManager;
            _signmanager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //将数据从模型中复制到IdentityUser
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    City = model.City
                };

                var result = await _usermanager.CreateAsync(user, model.Password);

                //如果注册成功，则使用登录服务登录
                //并重定向到HomeController的索引操作
                if (result.Succeeded)
                {
                    //生成电子游戏确认令牌
                    var token = await _usermanager.GenerateEmailConfirmationTokenAsync(user);

                    //生成电子邮箱的确认链接
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
                    _logger.Log(LogLevel.Warning, confirmationLink);

                    if (_signmanager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Admin");
                    }
                    //await _signmanager.SignInAsync(user, isPersistent: false);
                    //return RedirectToAction("Index", "Home");

                    ViewBag.ErrorTitle = "注册成功";
                    ViewBag.ErrorMessage = "在您登入系统前，我们已经给您发了一封邮件，需要您进行邮件验证，点击确认链接即可完成";
                    return View("Error");
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
        public async Task<IActionResult> Login(string returnUrl)
        {
            LoginViewModel loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signmanager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(loginViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            model.ExternalLogins = (await _signmanager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = await _usermanager.FindByEmailAsync(model.Email);
                if (user != null && !user.EmailConfirmed && (await _usermanager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "邮箱尚未验证");
                    return View(model);
                }
                var result = await _signmanager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);


                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {

                        return Redirect(returnUrl);

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

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signmanager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }


        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signmanager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"第三方登录提供程序错误：{remoteError}");
                return View("Login", model);
            }

            //从第三方登陆提供商获取关于用户的信息
            var info = await _signmanager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "加载第三方登录信息出错");
                return View("Login", model);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            ApplicationUser user = null;

            if (email != null)
            {
                //通过邮箱去查询用户是否存在
                user = await _usermanager.FindByEmailAsync(email);

                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "您的电子邮箱还未进行验证");
                    return View("Login", model);
                }
            }

            //如果用户之前已经登录过了，则会在AspNetUserLogins表产生对应的记录
            //这个时候就无需创建新的记录，直接使用当前记录登录系统
            var signInResult = await _signmanager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                //否则就表示表中没有记录，则创建一个

                //var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email != null)
                {

                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                        };

                        await _usermanager.CreateAsync(user);

                        var token = await _usermanager.GenerateEmailConfirmationTokenAsync(user);

                        var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
                        _logger.Log(LogLevel.Warning, confirmationLink);

                        ViewBag.ErrorTitle = "注册成功";
                        ViewBag.ErrorMessage = "在您登入系统前，我们已经给您发了一封邮件，需要您进行邮件验证，点击确认链接即可完成";
                        return View("Error");
                    }

                    await _usermanager.AddLoginAsync(user, info);
                    await _signmanager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                //如果获取不到用户邮箱，则重定向到错误视图
                ViewBag.ErrorTitle = $"我们无法从提供商：{info.LoginProvider}中解析到您的邮件地址";
                ViewBag.ErrorMessage = "请通过luchong1999@outlook.com寻求技术支持";

                return View("Error");
            }
        }


        /// <summary>
        /// 验证邮箱是否激活
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("index", "home");
            }

            var user = await _usermanager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"当前{userId}无效";
                return View("NotFound");
            }

            var result = await _usermanager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View();
            }

            ViewBag.ErrorTitle = "您的电子邮箱还未进行验证";
            return View("Error");
        }


        #region 激活邮箱
        [HttpGet]
        public IActionResult ActivateUserEmail()
        {
            return View();
        }

        public async Task<IActionResult> ActivateUserEmail(EmailAdressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _usermanager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    if (!await _usermanager.IsEmailConfirmedAsync(user))
                    {
                        var token = await _usermanager.GenerateEmailConfirmationTokenAsync(user);

                        var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
                        _logger.Log(LogLevel.Warning, confirmationLink);


                        ViewBag.Message = "如果您在我们系统有注册账户，我们已经发了邮件到您的邮箱中，请前往邮箱激活您的账户！";
                        return View("ActivateUserEmailConfirmation", ViewBag.Message);
                    }
                }
            }
            ViewBag.Message = "请确认邮箱是否存在异常，现在我们无法给您发送激活链接。";
            return View("ActivateUserEmailConfirmation", ViewBag.Message);
        }
        #endregion

        #region 忘记密码
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(EmailAdressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _usermanager.FindByEmailAsync(model.Email);

                //如果找到了用户并确认了电子邮箱
                if (user != null && await _usermanager.IsEmailConfirmedAsync(user))
                {
                    //生成电子令牌
                    var token = await _usermanager.GeneratePasswordResetTokenAsync(user);

                    var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token = token }, Request.Scheme);

                    _logger.Log(LogLevel.Warning, passwordResetLink);

                    return View("ForgotPasswordConfirmation");
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (email == null || token == null)
            {
                ModelState.AddModelError(string.Empty, "无效的密码重置令牌");

            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _usermanager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await _usermanager.ResetPasswordAsync(user, model.Token, model.Password);

                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }

                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, item.Description);
                    }
                    return View(model);
                }

                return View("ResetPasswordConfirmation");
            }

            return View(model);
        }



        #endregion
    }
}