using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Models
{
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]  //用于自行指定主键值，而不由数据库自动生成
        public int CourseId { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }

        public ICollection<StudentCourse> StudentCourse { get; set; }
    }
}
