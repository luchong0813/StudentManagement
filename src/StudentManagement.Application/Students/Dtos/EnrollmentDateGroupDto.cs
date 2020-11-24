using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Application.Students.Dtos
{
    public class EnrollmentDateGroupDto
    {
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}",ApplyFormatInEditMode =true)]
        public DateTime? EnrollmentDate { get; set; }


        public int StudentCount { get; set; }
    }
}
