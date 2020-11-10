using StudentManagement.Application.Dtos;
using StudentManagement.Application.Teachers.Dtos;
using StudentManagement.Infrastructure.Repositories;
using StudentManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace StudentManagement.Application.Teachers
{
    public class TeacherService : ITeacherService
    {
        private readonly IRepository<Teacher, int> _teacherRepository;

        public TeacherService(IRepository<Teacher, int> teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        public async Task<PageResultDto<Teacher>> GetPageTeacherList(GetTeacherInput input)
        {
            var query = _teacherRepository.GetAll();

            if (!string.IsNullOrEmpty(input.FilterText))
            {
                query = query.Where(t => t.Name.Contains(input.FilterText));
            }

            //统计数据总条数
            var count = query.Count();
            //根据需求排序
            query = query.OrderBy(input.Sorting).Skip((input.CurrentPage - 1) * input.MaxResultCount).Take(input.MaxResultCount);

            var models = await query.Include(a => a.OfficeLocation)   //加载导航属性==>OfficeLocation
                .Include(a => a.CourseAssignments)  //加载导航属性==>CourseAssignments
                .ThenInclude(a => a.Course)  //加载CourseAssignments的导航属性==>Course
                .ThenInclude(a => a.StudentCourse)  //加载Course的导航属性==>StudentCourse
                .ThenInclude(a => a.Student)  //加载StudentCourse的导航属性==>Student
                .Include(i => i.CourseAssignments)  //加载导航属性==>CourseAssignments
                .ThenInclude(i => i.Course)  //加载CourseAssignments的导航属性==>Course
                .ThenInclude(i => i.Department)  //加载CourseAssignments的导航属性==>Department
                .AsNoTracking().ToListAsync();

            var dtos = new PageResultDto<Teacher>
            {
                TotalCount = count,
                CurrentPage = input.CurrentPage,
                MaxResultCount = input.MaxResultCount,
                Data = models,
                FilterText = input.FilterText,
                Sorting = input.Sorting
            };

            return dtos;
        }
    }
}
