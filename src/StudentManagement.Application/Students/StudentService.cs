using Microsoft.EntityFrameworkCore;
using StudentManagement.Application.Dtos;
using StudentManagement.Infrastructure.Repositories;
using StudentManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace StudentManagement.Application.Students
{
    public class StudentService : IStudentService
    {
        private readonly IRepository<Student, int> _studentRepository;

        public StudentService(IRepository<Student, int> studentRepository)
        {
            _studentRepository = studentRepository;
        }

        //public  async Task<List<Student>> GetPaginatedResult(int currentPage, string searchString, string sortBy, int pageSize = 10)
        //{
        //    var query = _studentRepository.GetAll();
        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        query = query.Where(s => s.Name.Contains(searchString) || s.Email.Contains(searchString));

        //    }
        //    query = query.OrderBy(sortBy);

        //    return await query.Skip((currentPage - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
        //}

        public async Task<PageResultDto<Student>> GetPaginatedResult(GetStudentInput input)
        {
            var query = _studentRepository.GetAll();
            if (!string.IsNullOrEmpty(input.FilterText))
            {
                query = query.Where(s => s.Name.Contains(input.FilterText) || s.Email.Contains(input.FilterText));
            }
            var count = query.Count();
            query = query.OrderBy(input.Sorting).Skip((input.CurrentPage - 1) * input.MaxResultCount).Take(input.MaxResultCount);

            var models = await query.AsNoTracking().ToListAsync();

            var dtos = new PageResultDto<Student>
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
