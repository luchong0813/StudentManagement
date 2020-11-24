using StudentManagement.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Application.Courses.Dtos
{
    public class GetCourseInput:PageStoredAndFilteInput
    {
        public GetCourseInput()
        {
            Sorting = "CourseID";
            MaxResultCount = 3;
        }
    }
}
