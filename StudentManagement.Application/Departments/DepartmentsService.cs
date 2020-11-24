using StudentManagement.Application.Dtos;
using StudentManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentManagement.Infrastructure.Repositories;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Application.Departments.Dtos;

namespace StudentManagement.Application.Departments
{
    public class DepartmentsService : IDepartmentsService
    {
        private readonly IRepository<Department, int> _departmentRepository;

        public DepartmentsService(IRepository<Department, int> departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<PageResultDto<Department>> GetPagedDepartmentsList(GetDepartmentInput input)
        {
            var query = _departmentRepository.GetAll();
            if (!string.IsNullOrEmpty(input.FilterText))
            {
                query = query.Where(s => s.Name.Contains(input.FilterText));
            }
            var count = query.Count();
            query = query.OrderBy(input.Sorting).Skip((input.CurrentPage - 1) * input.MaxResultCount).Take(input.MaxResultCount);

            //将查询结果转换为集合加载到内存中
            var models = await query.Include(a => a.Administrator).AsNoTracking().ToListAsync();

            var dtos = new PageResultDto<Department>
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
