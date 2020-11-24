using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Models
{
    /// <summary>
    /// 课程设置分配
    /// </summary>
    public class CourseAssignment
    {
        public int TeacherId { get; set; }

        public int CourseId { get; set; }

        public Teacher Teacher { get; set; }

        public Course Course { get; set; }
    }
}
