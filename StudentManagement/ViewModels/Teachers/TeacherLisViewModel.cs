using StudentManagement.Application.Dtos;
using StudentManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.ViewModels.Teachers
{
    public class TeacherLisViewModel
    {
        public PageResultDto<Teacher> Teachers { get; set; }

        public List<Course> Courses { get; set; }

        public List<StudentCourse> StudentCourses { get; set; }

        /// <summary>
        /// 选中的教师ID
        /// </summary>
        public int SelectedId { get; set; }

        /// <summary>
        /// 选中的课程ID
        /// </summary>
        public int SelectedCourseId { get; set; }
    }
}
