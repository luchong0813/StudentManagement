using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using StudentManagement.Application.Dtos;
using StudentManagement.Application.Students;
using StudentManagement.Application.Students.Dtos;
using StudentManagement.Infrastructure.Repositories;
using StudentManagement.Models;
using StudentManagement.Security.CustomTokenProvider;
using StudentManagement.ViewModels;

namespace StudentManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Student, int> _studentRepository;
        private readonly IDataProtector _protector;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IStudentService _studentService;

        public HomeController(IRepository<Student, int> studentRepository, IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings, IWebHostEnvironment webHostEnvironment, IStudentService studentService)
        {
            _studentRepository = studentRepository;
            _protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.StudentIdRouteValue);
            _webHostEnvironment = webHostEnvironment;
            _studentService = studentService;
        }

        public async Task<IActionResult> Index(GetStudentInput input)
        {

            //获取分页结果
            var dtos = await _studentService.GetPaginatedResult(input);
            dtos.Data = dtos.Data.Select(s =>
            {
                s.EncryptedId = _protector.Protect(s.Id.ToString());
                return s;
            }).ToList();

            return View(dtos);
        }

        public IActionResult Details(string id)
        {
            //throw new Exception("人为的抛出一个异常");

            //string decryptedId = protector.Unprotect(id);
            //int decryptedStudentId = Convert.ToInt32(decryptedId);
            var student = DecryPtedStudent(id);

            //Student student = _studentRepository.GetStudent(decryptedStudentId);

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
            homeDetailsViewModel.Student.EncryptedId = _protector.Protect(student.Id.ToString());
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
                    Photo = uniqueFileName,
                    EnrollmentDate = model.EnrollmentDate
                };

                //_studentRepository.Add(newStudent);

                _studentRepository.Insert(newStudent);

                var encryptedId = _protector.Protect(newStudent.Id.ToString());

                return RedirectToAction("Details", new { id = encryptedId });
            }
            return View();
        }

        [HttpGet]
        public ViewResult Edit(string id)
        {
            //string decryptedId = protector.Unprotect(id);
            //int decryptedStudentId = Convert.ToInt32(decryptedId);

            //Student student = _studentRepository.GetStudent(id);
            var student = DecryPtedStudent(id);
            if (student == null)
            {
                Response.StatusCode = 404;
                return View("StudentNotFound", id);
            }

            StudentEditViewModel studentEditViewModel = new StudentEditViewModel()
            {
                Id = id,
                Name = student.Name,
                Email = student.Email,
                ClassName = student.ClassName,
                ExistPhontPath = student.Photo,
                EnrollmentDate = student.EnrollmentDate,
                EncryptedId = student.EncryptedId
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
                //Student student = _studentRepository.GetStudent(model.Id);
                var student = DecryPtedStudent(model.Id.ToString());

                student.Name = model.Name;
                student.Email = model.Email;
                student.ClassName = model.ClassName;
                student.EnrollmentDate = model.EnrollmentDate;

                if (model.Photo != null)
                {
                    if (model.ExistPhontPath != null)
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", model.ExistPhontPath);

                        //因为用户又上传了新的图片，所以为了避免占用资源，直接删掉原来的
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }

                    }

                    student.Photo = ProcessUploadFile(model);
                }
                Student updateStudent = _studentRepository.Update(student);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            //Student student = _studentRepository.GetStudent(id);
            var student = _studentRepository.FirstOrDefaultAsync(s => s.EncryptedId == id.ToString());
            if (student == null)
            {
                //_studentRepository.Delete(student.Id);
                ViewBag.ErrorMessage = $"学生ID：{id}的信息不存在，请重试！";
                return View("NotFound", id);
            }
            await _studentRepository.DeleteAsync(s => s.Id == id);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> About()
        {
            var data = from Student in _studentRepository.GetAll()
                       group Student by Student.EnrollmentDate into dategroup
                       select new EnrollmentDateGroupDto()
                       {
                           EnrollmentDate=dategroup.Key,
                           StudentCount=dategroup.Count()
                       };
            var dtos = await data.AsNoTracking().ToListAsync();
            return View(dtos);
        }


        private string ProcessUploadFile(StudentCreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photo != null)
            {
                //获取上传头像存放的路径
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
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

        /// <summary>
        /// 解密学生信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Student DecryPtedStudent(string id)
        {
            string decryptedId = _protector.Unprotect(id);
            int decryptedStudentId = Convert.ToInt32(decryptedId);

            Student student = _studentRepository.FirstOrDefault(s => s.Id == decryptedStudentId);
            return student;
        }


    }
}
