using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using StudentManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudentManagement.ViewModel.Department
{
    public class DepartmentCreateViewModel
    {
        /// <summary>
        /// 学院编号
        /// </summary>
        [Display(Name ="学院编号")]
        public int DepartmentId { get; set; }

        /// <summary>
        /// 学院名称
        /// </summary>
        [Display(Name = "学院名称")]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        /// <summary>
        /// 预算
        /// </summary>
        [DataType(DataType.Currency)]
        [Display(Name ="预算")]
        public decimal Budget { get; set; }

        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}",ApplyFormatInEditMode =true)]
        [DataType(DataType.DateTime)]
        [Display(Name ="成立时间")]
        public DateTime StartDate { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        [Display(Name ="负责人")]
        public SelectList TeacherList { get; set; }

        public int? TeacherId { get; set; }

        public Teacher Administrator { get; set; }
    }
}
