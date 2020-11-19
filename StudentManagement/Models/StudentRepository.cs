using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Models
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _DbContext;
        //private readonly List<Student> _studentList;

        public StudentRepository(AppDbContext appDbContext)
        {
            _DbContext = appDbContext;
            //_studentList = new List<Student>()
            //{
            //    new Student{ Name="鲁小冲",ClassName=ClassName.ClassFour,Email="luchong1999@outlook.com",EnrollmentDate=DateTime.Parse("2017-09-02")},
            //        new Student{ Name="刘小彤",ClassName=ClassName.ClassFour,Email="liuxiaotong@outlook.com",EnrollmentDate=DateTime.Parse("2017-09-02")},
            //        new Student{ Name="张小三",ClassName=ClassName.ClassFive,Email="zhangxiaosan@outlook.com",EnrollmentDate=DateTime.Parse("2018-09-01")},
            //        new Student{ Name="李小四",ClassName=ClassName.ClassSix,Email="lixiaosi@outlook.com",EnrollmentDate=DateTime.Parse("2019-09-02")},
            //        new Student{ Name="王小五",ClassName=ClassName.ClassFive,Email="wangxiaowu@outlook.com",EnrollmentDate=DateTime.Parse("2020-09-01")}
            //};
        }

        public Student Add(Student student)
        {
            _DbContext.Add(student);
            _DbContext.SaveChanges();
            return student;
            //student.Id = _studentList.Max(s => s.Id) + 1;
            //_studentList.Add(student);
            //return student;
        }

        public Student Delete(int id)
        {
            var student = _DbContext.students.Find(id);
            if (student != null)
            {
                _DbContext.students.Remove(student);
                _DbContext.SaveChanges();
            }
            //var student = _studentList.FirstOrDefault(s => s.Id == id);
            //if (student != null)
            //{
            //    _studentList.Remove(student);
            //    _DbContext.SaveChanges();
            //}
            return student;
        }

        public IEnumerable<Student> GetAllStudents()
        {
            return _DbContext.students;
            //return _studentList;
        }

        public Student GetStudent(int id)
        {
            return _DbContext.students.Find(id);
            //return _studentList.FirstOrDefault(s => s.Id == id);
        }

        public Student Update(Student updateStudent)
        {
            var student = _DbContext.students.Attach(updateStudent);
            student.State = EntityState.Modified;
            _DbContext.SaveChanges();
            return updateStudent;

            
        }
    }
}
