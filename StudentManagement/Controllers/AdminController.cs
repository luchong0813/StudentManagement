using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        #endregion
    }
}
