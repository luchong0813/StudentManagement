using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentManagement.Application.Departments.Dtos;
using StudentManagement.Application.Dtos;
using StudentManagement.Models;

namespace StudentManagement.Application.Departments
{
    public interface IDepartmentsService
    {
        Task<PageResultDto<Department>> GetPagedDepartmentsList(GetDepartmentInput input);
    }
}
