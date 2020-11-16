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
        private readonly IRepository<CourseAssignment, int> _courseassignmentRepository;

        public CourseController(ICourseService courseService, IRepository<Department, int> departmentRepository, IRepository<Course, int> courseRepositry, IRepository<CourseAssignment, int> courseassignmentRepository)
        {
            _courseService = courseService;
            _departmentRepository = departmentRepository;
            _courseRepositry = courseRepositry;
            _courseassignmentRepository = courseassignmentRepository;
        }

        public async Task<IActionResult> Index(GetCourseInput input)
        {
            var models = await _courseService.GetPaginatedResult(input);
            return View(models);
        }

        #region 添加课程

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
                    DepartmentId = inputModel.DepartmentId
                };
                await _courseRepositry.InsertAsync(course);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }


        #endregion

        #region 编辑课程
        public IActionResult Edit(int? courseId)
        {
            if (!courseId.HasValue)
            {
                ViewBag.ErrorMessage = $"课程编号{courseId}的信息不存在，请重试！";
                return View("NotFound");
            }
            var course = _courseRepositry.FirstOrDefault(c => c.CourseId == courseId);
            if (course == null)
            {
                ViewBag.ErrorMessage = $"课程编号{courseId}的信息不存在，请重试！";
                return View("NotFound");
            }
            var dtos = DepartmentsDropDownList(course.DepartmentId);
            CourseCreateViewModel courseCreateViewModel = new CourseCreateViewModel
            {
                DepartmentList = dtos,
                CourseID = course.CourseId,
                Credits = course.Credits,
                Title = course.Title,
                DepartmentId = course.DepartmentId
            };
            return View(courseCreateViewModel);
        }

        [HttpPost]
        public IActionResult Edit(CourseCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var course = _courseRepositry.FirstOrDefault(c => c.CourseId == model.CourseID);
                if (course == null)
                {
                    ViewBag.ErrorMessage = $"课程编号{model.CourseID}的信息不存在，请重试！";
                    return View("NotFound");
                }
                course.CourseId = model.CourseID;
                course.Credits = model.Credits;
                course.DepartmentId = model.DepartmentId;
                course.Title = model.Title;
                _courseRepositry.Update(course);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        #endregion

        #region 课程详情
        public async Task<IActionResult> Details(int courseId)
        {
            //Include：预加载导航属性
            var course = await _courseRepositry.GetAll().Include(d => d.Department).FirstOrDefaultAsync(c => c.CourseId == courseId);
            if (course == null)
            {
                ViewBag.ErrorMessage = $"课程编号{courseId}的信息不存在，请重试！";
                return View("NotFound");
            }
            return View(course);
        }

        #endregion

        #region 删除课程
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _courseRepositry.FirstOrDefaultAsync(c => c.CourseId == id);
            if (model == null)
            {
                ViewBag.ErrorMessage = $"课程编号{id}的信息不存在，请重试！";
                return View("NotFound");
            }
            await _courseassignmentRepository.DeleteAsync(c => c.CourseId == model.CourseId);
            await _courseRepositry.DeleteAsync(c => c.CourseId == id);
            return RedirectToAction(nameof(Index));
        }
        #endregion

        /// <summary>
        /// 学院下拉列表
        /// </summary>
        /// <param name="selectedDepartment"></param>
        /// <returns></returns>
        private SelectList DepartmentsDropDownList(object selectedDepartment = null)
        {
            var models = _departmentRepository.GetAll().OrderBy(d => d.Name).AsNoTracking().ToList();
            var dtos = new SelectList(models, "DepartmentId", "Name", selectedDepartment);
            return dtos;
        }
    }
}
