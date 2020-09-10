using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Models;
using StudentManagement.ViewModels;

namespace StudentManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStudentRepository _studentRepository;

        public HomeController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public IActionResult Index()
        {
            var students = _studentRepository.GetAllStudents();
            return View(students);
        }

        public IActionResult Details(int? id)
        {

            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                PageTiltle = "学生详情",
                Student = _studentRepository.GetStudent(id ?? 1)
            };

            return View(homeDetailsViewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Student student)
        {
            if (ModelState.IsValid)
            {
                var stu = _studentRepository.Add(student);
                //return RedirectToAction("Details", new { id = stu.Id });
            }
            return View();
        }
    }
}
