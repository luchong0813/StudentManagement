using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace StudentManagement.Controllers
{
    public class CourseController : Controller
    {

        public CourseController( )
        {

        }

        public ActionResult Index()
        {
            return View();
        }
    }
}
