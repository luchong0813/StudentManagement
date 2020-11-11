using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Application.Courses;
using StudentManagement.Application.Courses.Dtos;
using StudentManagement.Infrastructure.Repositories;
using StudentManagement.Models;

namespace StudentManagement.Controllers
{
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IRepository<Department, int> _departmentRepository;
        private readonly IRepository<Course, int> _courseRepositry;

        public CourseController(ICourseService courseService, IRepository<Department, int> departmentRepository,IRepository<Course,int> courseRepositry)
        {
            _courseService = courseService;
            _departmentRepository = departmentRepository;
            _courseRepositry = courseRepositry;
        }

        public async Task<IActionResult> Index(GetCourseInput input)
        {
            var models = await _courseService.GetPaginatedResult(input);
            return View(models);
        }

        #region 添加课程
        [HttpGet]
        public ActionResult Create()
        {
            var dtos = DepartmentsDropDownList();
            CourseCreateViewModel courseCreateViewModel = new CourseCreateViewModel
            {
                DepartmentList = dtos
            };
            return View(courseCreateViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CourseCreateViewModel inputModel)
        {
            if (ModelState.IsValid)
            {
                Course course = new Course
                {
                    CourseId = inputModel.CourseID,
                    Title = inputModel.Title,
                    Credits = inputModel.Credits,
                    DepartmentId = inputModel.DepartmentID
                };
                await _courseRepositry.InsertAsync(course);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        /// <summary>
        /// 学院下拉列表
        /// </summary>
        /// <param name="selectedDepartment"></param>
        /// <returns></returns>
        private SelectList DepartmentsDropDownList(object selectedDepartment = null)
        {
            var models = _departmentRepository.GetAll().OrderBy(d => d.Name).AsNoTracking().ToList();
            var dtos = new SelectList(models, "DepartmentID", "Name", selectedDepartment);
            return dtos;
        }
        #endregion
    }
}
