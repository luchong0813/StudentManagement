using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Models;
using StudentManagement.ViewModels;

namespace StudentManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        #region 角色管理
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole()
                {
                    Name = model.RoleName
                };

                IdentityResult result = await _roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Admin");
                }

                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View(model);
        }

        /// <summary>
        /// 查询所有角色
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }


        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRole(string id)
        {
            //通过TagHelper传进来的ID查询角色信息
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"角色ID：{id}的信息不存在，请重试！";
                return View("NotFound");
            }

            var model = new EditRoleViewModel()
            {
                Id = role.Id,
                RoleName = role.Name
            };

            var users = _userManager.Users.ToList();

            //查询所有用户
            foreach (var item in users)
            {
                if (await _userManager.IsInRoleAsync(item, role.Name))
                {
                    model.Users.Add(item.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"角色ID：{model.Id}的信息不存在，请重试！";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;

                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var user = await _roleManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"角色ID：{id}的信息不存在，请重试！";
                return View("NotFound");
            }
            else
            {
                try
                {
                    var result = await _roleManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }

                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }

                    return View("ListRoles");
                }
                catch (DbUpdateException ex)
                {
                    ViewBag.ErrorTitle = $"角色：{user.Name}正在被使用中!";
                    ViewBag.ErrorMessage = $"无法删除{user.Name}角色，因为此角色下还存在用户。如果您想删除此角色，需要先删除该角色下所有用户，然后再次尝试删除该角色本身。";
                    return View("Error");
                }

            }

        }

        #endregion

        #region 角色中的用户
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"角色Id：{roleId}的信息不存在，请重试";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();
            var users = _userManager.Users.ToList();

            foreach (var item in users)
            {
                var userRoleViewModel = new UserRoleViewModel()
                {
                    UserId = item.Id,
                    UserName = item.UserName
                };

                //判断当前用户是否已经存在在该角色中
                bool isInRole = await _userManager.IsInRoleAsync(item, role.Name);

                if (isInRole)
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }
                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            //检查角色是否存在
            if (role == null)
            {
                ViewBag.ErrorMessage = $"角色Id：{roleId}的信息不存在，请重试";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;

                //检查当前的userId是否被选中，如果是则添加到角色列表中
                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    //判断当前用户是否是最后一个用户，如果是则跳转回视图，不是则进入下一个循环
                    if (i < model.Count - 1)
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("EditRole", new { Id = roleId });
                    }
                }
            }
            return RedirectToAction("EditRole", new { Id = roleId });
        }
        #endregion

        #region 用户管理
        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage($"无法找到ID为：{id}的用户");
                return View("NotFound", id);
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                City = user.City,
                Claims = claims.Select(c => c.Value).ToList(),
                //Claims = claims,
                Roles = roles.ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                ViewBag.ErrorMessage($"无法找到ID为：{model.Id}的用户");
                return View("NotFound");
            }
            else
            {
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.City = model.City;
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("ListUsers", "Admin");
            }

            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item.Description);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage($"无法找到ID为：{id}的用户");
                return View("NotFound");
            }
            else
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
            }
            return View("ListUsers");
        }

        [HttpGet]
        //[Authorize(Policy ="EditRolePolicy")]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage($"无法找到ID为：{userId}的用户");
                return View("NotFound");
            }

            var model = new List<RolesInUserViewModel>();
            var roles = await _roleManager.Roles.ToListAsync();
            foreach (var item in roles)
            {
                var rolesInUserViewModel = new RolesInUserViewModel()
                {
                    RoleId = item.Id,
                    RoleName = item.Name
                };
                //判断当前用户是否已经在该角色中
                if (await _userManager.IsInRoleAsync(user, item.Name))
                {
                    //将已拥有的角色信息设置为选中
                    rolesInUserViewModel.IsSelected = true;
                }
                else
                {
                    rolesInUserViewModel.IsSelected = false;
                }
                model.Add(rolesInUserViewModel);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<RolesInUserViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage($"无法找到ID为：{userId}的用户");
                return View("NotFound");
            }


            var roles = await _userManager.GetRolesAsync(user);
            //移除当前用户中的所有角色信息
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "无法删除用户中现有的角色");
                return View(model);
            }
            //查询列表中被选中的RoleName并添加到用户中
            result = await _userManager.AddToRolesAsync(user, model.Where(x => x.IsSelected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "无法向用户添加选定的角色");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = userId });
        }

        #endregion

        #region 管理用户声明
        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage($"无法找到ID为：{userId}的用户");
                return View("NotFound", userId);
            }

            //UserManager服务中的GetClaimsAsync()方法获取用户当前的所有声明
            var existingUserClaims = await _userManager.GetClaimsAsync(user);

            var model = new UserClaimsViewModel
            {
                UserId = userId
            };

            foreach (var item in ClaimsStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim()
                {
                    ClaimType = item.Type
                };

                //如果用户选中了声明属性，则设置IsSelected为True
                if (existingUserClaims.Any(c => c.Type == item.Type))
                {
                    userClaim.IsSelected = true;
                }
                model.Cliams.Add(userClaim);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage($"无法找到ID为：{model.UserId}的用户");
                return View("NotFound", model.UserId);
            }

            //获取所有用户现有的声明并删除它们
            var claims = await _userManager.GetClaimsAsync(user);
            var result = await _userManager.RemoveClaimsAsync(user, claims);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "无法删除当前用户的声明");
                return View(model);
            }

            // 添加页面上所有选中的声明
            //result = await _userManager.AddClaimsAsync(user, model.Cliams.Where(c => c.IsSelected).Select(c => new Claim(c.ClaimType, c.ClaimType)));

            //添加Claim列表到数据库中，然后对UI界面上被选中的值进行bool判单
            result = await _userManager.AddClaimsAsync(user, model.Cliams.Select(c => new Claim(c.ClaimType, c.IsSelected ? "true" : "false")));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "无法向用户添加选定的声明");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = model.UserId });
        }
        #endregion

        #region 视图授权
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
        #endregion
    }
}
