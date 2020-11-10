using StudentManagement.Application.Dtos;
using StudentManagement.Application.Teachers.Dtos;
using StudentManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Application.Teachers
{
    public interface ITeacherService
    {
        /// <summary>
        /// 获取教师的分页信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PageResultDto<Teacher>> GetPageTeacherList(GetTeacherInput input);
    }
}
