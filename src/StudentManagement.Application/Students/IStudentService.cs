using StudentManagement.Application.Dtos;
using StudentManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Application.Students
{
    public interface IStudentService
    {
        //Task<List<Student>> GetPaginatedResult(int currentPage, string searchString, string sortBy, int pageSize = 10);

        Task<PageResultDto<Student>> GetPaginatedResult(GetStudentInput input);
    }
}
