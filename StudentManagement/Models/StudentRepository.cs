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

        public StudentRepository(AppDbContext appDbContext)
        {
            _DbContext = appDbContext;
        }

        public Student Add(Student student)
        {
            _DbContext.Add(student);
            _DbContext.SaveChanges();
            return student;
        }

        public Student Delete(int id)
        {
            var student = _DbContext.students.Find(id);
            if (student != null)
            {
                _DbContext.students.Remove(student);
                _DbContext.SaveChanges();
            }
            return student;
        }

        public IEnumerable<Student> GetAllStudents()
        {
            return _DbContext.students;
        }

        public Student GetStudent(int id)
        {
            return _DbContext.students.Find(id);
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
