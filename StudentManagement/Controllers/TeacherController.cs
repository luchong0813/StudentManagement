using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Application.Teachers;
using StudentManagement.Application.Teachers.Dtos;
using StudentManagement.Infrastructure.Repositories;
using StudentManagement.Models;
using StudentManagement.ViewModels.Teachers;

namespace StudentManagement.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ITeacherService _teacherService;
        private readonly IRepository<Teacher, int> _teacherRepository;
        private readonly IRepository<Course, int> _courseRepository;
        private readonly IRepository<OfficeLocation, int> _officelocationRepository;
        private readonly IRepository<CourseAssignment, int> _courseassigmentRepository;

        public TeacherController(ITeacherService teacherService, IRepository<Teacher, int> teacherRepository, IRepository<Course, int> courseRepository, IRepository<OfficeLocation, int> officelocationRepository, IRepository<CourseAssignment, int> courseassigmentRepository)
        {
            _teacherService = teacherService;
            _teacherRepository = teacherRepository;
            _courseRepository = courseRepository;
            _officelocationRepository = officelocationRepository;
            _courseassigmentRepository = courseassigmentRepository;
        }

        public async Task<IActionResult> Index(GetTeacherInput input)
        {
            var models = await _teacherService.GetPageTeacherList(input);
            var dto = new TeacherLisViewModel();
            if (input.Id != null)
            {
                var teacher = models.Data.FirstOrDefault(t => t.Id == input.Id.Value);
                if (teacher != null)
                {
                    dto.Courses = teacher.CourseAssignments.Select(c => c.Course).ToList();
                }
                dto.SelectedCourseId = input.Id.Value;
            }

            if (input.CourseId.HasValue)  //当属性为可空类型时则可以使用HasValue，与 !=null 同理
            {
                //查询该课程下有多少学生报名
                var course = dto.Courses.FirstOrDefault(c => c.CourseId == input.CourseId.Value);
                if (course != null)
                {
                    dto.StudentCourses = course.StudentCourse.ToList();
                }
                dto.SelectedCourseId = input.CourseId.Value;
            }
            dto.Teachers = models;
            return View(dto);
        }

        #region 编辑教师
        public async Task<IActionResult> Edit(int? id)
        {
            var model = await _teacherRepository.GetAll()
                .Include(t => t.OfficeLocation)
                .Include(t => t.CourseAssignments)
                .ThenInclude(c => c.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (model == null)
            {
                ViewBag.ErrorMessage = $"教师信息Id：{id}的信息不存在，请重试！";
                return View("NotFound");
            }

            var dto = new TeacherCreateViewModel
            {
                Name = model.Name,
                HireDate = model.HireDate,
                Id = model.Id,
                OfficeLocation = model.OfficeLocation
            };

            dto.AssignedCourses = AssignedCourseDroupDownList(model);
            return View(dto);
        }

        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> Edit(TeacherCreateViewModel input)
        {
            if (ModelState.IsValid)
            {
                var teacher = await _teacherRepository.GetAll()
                    .Include(o => o.OfficeLocation)
                    .Include(i => i.CourseAssignments)
                    .ThenInclude(c => c.Course)
                    .FirstOrDefaultAsync(m => m.Id == input.Id);

                if (teacher == null)
                {
                    ViewBag.ErrorMessage = $"教师信息Id：{input.Id}的信息不存在，请重试！";
                    return View("NotFound");
                }

                teacher.HireDate = input.HireDate;
                teacher.Name = input.Name;
                teacher.OfficeLocation = input.OfficeLocation;
                teacher.CourseAssignments = new List<CourseAssignment>();

                var course = input.AssignedCourses.Where(a => a.IsSelected == true).ToList();

                foreach (var item in course)
                {
                    teacher.CourseAssignments.Add(new CourseAssignment
                    {
                        CourseId = item.CourseId,
                        TeacherId = teacher.Id
                    });
                }
                await _teacherRepository.UpdateAsync(teacher);
                return RedirectToAction(nameof(Index));
            }
            return View(input);
        }
        #endregion

        #region 添加教师
        public ActionResult Create()
        {
            var allCourse = _courseRepository.GetAllList();
            var viewModel = new List<AssignedCourseViewModel>();
            foreach (var item in allCourse)
            {
                viewModel.Add(new AssignedCourseViewModel
                {
                    CourseId = item.CourseId,
                    Title = item.Title,
                    IsSelected = false
                });
            }
            var dto = new TeacherCreateViewModel();
            dto.AssignedCourses = viewModel;
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TeacherCreateViewModel input)
        {
            if (ModelState.IsValid)
            {
                var teacher = new Teacher
                {
                    HireDate = input.HireDate,
                    Name = input.Name,
                    OfficeLocation = input.OfficeLocation,
                    CourseAssignments = new List<CourseAssignment>()
                };
                //获取用户选中的课程信息
                var courses = input.AssignedCourses.Where(a => a.IsSelected == true).ToList();
                foreach (var item in courses)
                {
                    teacher.CourseAssignments.Add(new CourseAssignment
                    {
                        CourseId = item.CourseId,
                        TeacherId = teacher.Id
                    });
                }
                await _teacherRepository.InsertAsync(teacher);
                return RedirectToAction(nameof(Index));
            }
            return View(input);
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _teacherRepository.FirstOrDefaultAsync(t => t.Id == id);
            if (model == null)
            {
                ViewBag.ErrorMessage = $"教师信息Id：{id}的信息不存在，请重试！";
                return View("NotFound");
            }
            await _officelocationRepository.DeleteAsync(o => o.TeacherId == model.Id);
            await _courseassigmentRepository.DeleteAsync(a => a.TeacherId == model.Id);
            await _teacherRepository.DeleteAsync(a => a.Id == id);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// 判断课程列表是否被选中
        /// </summary>
        /// <param name="teacher"></param>
        /// <returns></returns>
        private List<AssignedCourseViewModel> AssignedCourseDroupDownList(Teacher teacher)
        {
            //获取课程列表
            var allCourse = _courseRepository.GetAllList();
            //获取教师当前教授的课程
            var teacherCourses = new HashSet<int>(teacher.CourseAssignments.Select(c => c.CourseId));

            var model = new List<AssignedCourseViewModel>();

            foreach (var item in allCourse)
            {
                model.Add(new AssignedCourseViewModel
                {
                    CourseId = item.CourseId,
                    Title = item.Title,
                    IsSelected = teacherCourses.Contains(item.CourseId)
                });
            }
            return model;
        }
    }
}
