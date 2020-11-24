using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Models
{
    /// <summary>
    /// 课程
    /// </summary>
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]  //用于自行指定主键值，而不由数据库自动生成
        [Display(Name = "课程编号")]
        public int CourseId { get; set; }

        [Display(Name ="课程名称")]
        public string Title { get; set; }

        [Display(Name = "课程学分")]
        [Range(0,5)]
        public int Credits { get; set; }

        public int DepartmentId { get; set; }

        public Department Department { get; set; }

        public ICollection<CourseAssignment> CourseAssignments { get; set; }

        public ICollection<StudentCourse> StudentCourse { get; set; }
    }
}
