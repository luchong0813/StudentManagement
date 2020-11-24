using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Application.Dtos
{
    public class GetStudentInput : PageStoredAndFilteInput
    {
        public GetStudentInput()
        {
            Sorting = "Id";
        }
    }
}
