using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentManagement.Application.Dtos;

namespace StudentManagement.Application.Departments.Dtos
{
    public class GetDepartmentInput : PageStoredAndFilteInput
    {
        public GetDepartmentInput()
        {
            Sorting = "Name";
            MaxResultCount = 3;
        }
    }
}
