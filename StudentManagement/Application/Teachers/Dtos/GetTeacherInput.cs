using StudentManagement.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Application.Teachers.Dtos
{
    public class GetTeacherInput : PageStoredAndFilteInput
    {
        public int? Id { get; set; }

        public int? CourseId { get; set; }

        public GetTeacherInput()
        {
            Sorting = "Id";
            MaxResultCount = 3;
        }
    }
}
