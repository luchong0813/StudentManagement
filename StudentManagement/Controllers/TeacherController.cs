using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Application.Teachers;
using StudentManagement.Application.Teachers.Dtos;
using StudentManagement.ViewModels.Teachers;

namespace StudentManagement.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
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
    }
}
