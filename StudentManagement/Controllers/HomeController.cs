using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Models;
using StudentManagement.Security.CustomTokenProvider;
using StudentManagement.ViewModels;

namespace StudentManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IDataProtector protector;

        public HomeController(IStudentRepository studentRepository, IWebHostEnvironment webHostEnvironment, IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            _studentRepository = studentRepository;
            this.webHostEnvironment = webHostEnvironment;
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.StudentIdRouteValue);
        }

        public IActionResult Index()
        {
            List<Student> model = _studentRepository.GetAllStudents().Select(s =>
            {
                s.EncryptedId = protector.Protect(s.Id.ToString());
                return s;
            }).ToList();
            //var students = _studentRepository.GetAllStudents();
            return View(model);
        }

        public IActionResult Details(string id)
        {
            //throw new Exception("人为的抛出一个异常");

            string decryptedId = protector.Unprotect(id);
            int decryptedStudentId = Convert.ToInt32(decryptedId);

            Student student = _studentRepository.GetStudent(decryptedStudentId);

            if (student == null)
            {
                ViewBag.ErrorMessage = $"学生ID：{id}的信息不存在，请重试！";
                return View("NotFound", id);
            }

            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                PageTiltle = "学生详情",
                Student = student
            };

            //ViewBag.EncryptedId = protector.Protect(decryptedStudentId.ToString());

            return View(homeDetailsViewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(StudentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var uniqueFileName = ProcessUploadFile(model);

                //多文件上传
                //if (model.Photos != null && model.Photos.Count() > 1)
                //{
                //    foreach (var photo in model.Photos)
                //    {
                //        //获取上传头像存放的路径
                //        string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                //        //生成唯一的文件名s
                //        uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                //        string filePath = Path.Combine(uploadFolder, uniqueFileName);

                //        photo.CopyTo(new FileStream(filePath, FileMode.Create));
                //    }
                //}


                Student newStudent = new Student()
                {
                    Name = model.Name,
                    Email = model.Email,
                    ClassName = model.ClassName,
                    Photo = uniqueFileName
                };

                _studentRepository.Add(newStudent);
                return RedirectToAction("Details", new { id = newStudent.Id });
            }
            return View();
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            //string decryptedId = protector.Unprotect(id);
            //int decryptedStudentId = Convert.ToInt32(decryptedId);

            Student student = _studentRepository.GetStudent(id);

            if (student == null)
            {
                Response.StatusCode = 404;
                return View("StudentNotFound", id);
            }

            StudentEditViewModel studentEditViewModel = new StudentEditViewModel()
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                ClassName = student.ClassName,
                ExistPhontPath = student.Photo,
            };



            return View(studentEditViewModel);
        }

        [HttpPost]
        public IActionResult Edit(StudentEditViewModel model)
        {
           

            //检查提供的数据是否有效
            if (ModelState.IsValid)
            {
                //从数据库查询正在编辑的学生信息
                Student student = _studentRepository.GetStudent(model.Id);

                student.Name = model.Name;
                student.Email = model.Email;
                student.ClassName = model.ClassName;

                if (model.Photo != null)
                {
                    if (model.ExistPhontPath != null)
                    {
                        string filePath = Path.Combine(webHostEnvironment.WebRootPath, "images", model.ExistPhontPath);

                        //因为用户又上传了新的图片，所以为了避免占用资源，直接删掉原来的
                        System.IO.File.Delete(filePath);
                    }

                    student.Photo = ProcessUploadFile(model);
                }
                Student updateStudent = _studentRepository.Update(student);
                return RedirectToAction("Index");
            }
            return View(model);
        }


        public IActionResult Delete(int id)
        {
            Student student = _studentRepository.GetStudent(id);

            if (student != null)
            {
                _studentRepository.Delete(student.Id);
            }
            return RedirectToAction("Index");
        }


        private string ProcessUploadFile(StudentCreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photo != null)
            {
                //获取上传头像存放的路径
                string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                //生成唯一的文件名
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                //model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));

                //因为使用了非托管的资源，所以需要手动释放（凡是派生自IDispose的都需要手动释放）
                using var fileStream = new FileStream(filePath, FileMode.Create);
                model.Photo.CopyTo(fileStream);
            }

            return uniqueFileName;
        }
    }
}
