using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentManagement.Infrastructure.Repositories;
using StudentManagement.Models;
using StudentManagement.Application.Departments;
using StudentManagement.Application.Dtos;
using StudentManagement.Application.Departments.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagement.ViewModel.Department;

namespace StudentManagement.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IRepository<Department, int> _departmentRepository;
        private readonly IDepartmentsService _departmentsService;
        private readonly IRepository<Teacher, int> _teacherRepository;
        private readonly AppDbContext _dbContext;

        public DepartmentController(IRepository<Department, int> departmentRepository, IDepartmentsService departmentsService, IRepository<Teacher, int> teacherRepository, AppDbContext dbContext)
        {
            _departmentRepository = departmentRepository;
            _departmentsService = departmentsService;
            _teacherRepository = teacherRepository;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index(GetDepartmentInput input)
        {
            var models = await _departmentsService.GetPagedDepartmentsList(input);
            return View(models);
        }

        #region 创建学院
        public ActionResult Create()
        {
            var dto = new DepartmentCreateViewModel
            {
                TeacherList = TeachersDropDownList()
            };
            return View(dto);
        }


        [HttpPost]
        public async Task<IActionResult> Create(DepartmentCreateViewModel input)
        {
            if (ModelState.IsValid)
            {
                Department department = new Department
                {
                    StartDate = input.StartDate,
                    DepartmentId = input.DepartmentId,
                    TeacherId = input.TeacherId,
                    Budget = input.Budget,
                    Name = input.Name
                };
                await _departmentRepository.InsertAsync(department);
                return RedirectToAction(nameof(Index));
            }
            return View(input);
        }
        #endregion

        #region 学院详情
        public async Task<IActionResult> Details(int id)
        {
            var model = await _departmentRepository.GetAll().Include(a => a.Administrator).FirstOrDefaultAsync(a => a.DepartmentId == id);
            if (model == null)
            {
                ViewBag.ErrorMessage = $"学院信息Id为{id}的信息不存在！";
                return View("NotFound");
            }
            return View(model);
        }
        #endregion

        public async Task<IActionResult> Delete(int id)
        {
            var model = await _departmentRepository.FirstOrDefaultAsync(a => a.DepartmentId == id);
            if (model == null)
            {
                ViewBag.ErrorMessage = $"学院信息Id为{id}的信息不存在！";
                return View("NotFound");
            }
            await _departmentRepository.DeleteAsync(a => a.DepartmentId == id);
            return RedirectToAction(nameof(Index));
        }

        #region 编辑学院
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _departmentRepository.GetAll().Include(a => a.Administrator).AsNoTracking().FirstOrDefaultAsync(a => a.DepartmentId == id);
            if (model == null)
            {
                ViewBag.ErrorMessage = $"学院信息Id为{id}的信息不存在！";
                return View("NotFound");
            }
            var teacherList = TeachersDropDownList();
            var dto = new DepartmentCreateViewModel
            {
                DepartmentId = model.DepartmentId,
                Name = model.Name,
                StartDate = model.StartDate,
                Budget = model.Budget,
                TeacherId = model.TeacherId,
                Administrator = model.Administrator,
                RowVersion = model.RowVersion,
                TeacherList = teacherList
            };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DepartmentCreateViewModel input)
        {
            if (ModelState.IsValid)
            {
                var model = await _departmentRepository.GetAll().Include(a => a.Administrator).FirstOrDefaultAsync(a => a.DepartmentId == input.DepartmentId);
                if (model == null)
                {
                    ViewBag.ErrorMessage = $"学院信息Id为{input.DepartmentId}的信息不存在！";
                    return View("NotFound");
                }
                model.DepartmentId = input.DepartmentId;
                model.Name = input.Name;
                model.Budget = input.Budget;
                model.Administrator = input.Administrator;
                model.StartDate = input.StartDate;
                model.TeacherId = input.TeacherId;

                await _departmentRepository.UpdateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(input);
        }
        #endregion

        /// <summary>
        /// 教师下拉列表
        /// </summary>
        /// <param name="selectedTeacher"></param>
        /// <returns></returns>
        private SelectList TeachersDropDownList(object selectedTeacher = null)
        {
            var models = _teacherRepository.GetAll().OrderBy(a => a.Name).AsNoTracking().ToList();
            var dtos = new SelectList(models, "Id", "Name", selectedTeacher);
            return dtos;
        }
    }
}
