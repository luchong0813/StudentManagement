using StudentManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Application.Dtos
{
    public class PaginationModel
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage { get; set; } = 1;

        /// <summary>
        /// 总条数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 每页分页条数
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));

        public List<Student> Data { get; set; }

        //上一页
        public bool ShowPrevious => CurrentPage > 1;
        //下一页
        public bool ShowNext => CurrentPage < TotalPages;
        //返回第一页
        public bool ShowFirst => CurrentPage != 1;
        //返回最后一页
        public bool ShowLast => CurrentPage != TotalPages;
    }
}
