using StudentManagement.Application.Courses.Dtos;
using StudentManagement.Application.Dtos;
using StudentManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Application.Courses
{
    public interface ICourseService
    {
        Task<PageResultDto<Course>> GetPaginatedResult(GetCourseInput input);
    }
}
