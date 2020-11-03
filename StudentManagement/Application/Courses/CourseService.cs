using Microsoft.EntityFrameworkCore;
using StudentManagement.Application.Courses.Dtos;
using StudentManagement.Application.Dtos;
using StudentManagement.Infrastructure.Repositories;
using StudentManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace StudentManagement.Application.Courses
{
    public class CourseService : ICourseService
    {
        private readonly IRepository<Course, int> _courseRepository;

        public CourseService(IRepository<Course, int> courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<PageResultDto<Course>> GetPaginatedResult(GetCourseInput input)
        {
            var query = _courseRepository.GetAll();
            //统计查询数据的总条数，用于分页计算总页数
            var count = query.Count();
            //根据需求进行排序，然后进行分页逻辑的计算
            query = query.OrderBy(input.Sorting).Skip((input.CurrentPage - 1) * input.MaxResultCount).Take(input.MaxResultCount);
            //将查询结果转换为List集合，加载到内存中
            var models = await query.AsNoTracking().ToListAsync();

            var dtos = new PageResultDto<Course>
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
