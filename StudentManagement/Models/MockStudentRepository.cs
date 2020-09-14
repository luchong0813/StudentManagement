using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Models
{
    public class MockStudentRepository : IStudentRepository
    {
        private List<Student> _students;

        public MockStudentRepository()
        {
            _students = new List<Student>()
            {
                new Student{ Id=1,Name="张三",ClassName=ClassName.ClassFour,Email="84512211@outlook.com"},
                new Student{ Id=2,Name="李四",ClassName=ClassName.ClassSix,Email="451515jshjd@outlook.com"},
                new Student{ Id=3,Name="王五",ClassName=ClassName.ClassFour,Email="sghdha52@qq.com"},
                new Student{ Id=4,Name="赵六",ClassName=ClassName.ClassSix,Email="45xshxdjn22@outlook.com"},
                new Student{ Id=5,Name="鲁班",ClassName=ClassName.ClassFive,Email="dxshjc1515251@163.com"}
            };
        }

        public Student Add(Student student)
        {
            student.Id = _students.Max(s => s.Id) + 1;
            _students.Add(student);
            return student;
        }

        public Student Delete(int id)
        {
            var student = _students.FirstOrDefault(s => s.Id == id);
            if (student != null)
            {
                _students.Remove(student);
            }
            return student;
        }

        public IEnumerable<Student> GetAllStudents()
        {
            return _students;
        }

        public Student GetStudent(int id)
        {
            return _students.FirstOrDefault(s => s.Id == id);
        }

        public Student Update(Student updateStudent)
        {
            var student = _students.FirstOrDefault(s => s.Id == updateStudent.Id);
            if (student != null)
            {
                student.Name = updateStudent.Name;
                student.Email = updateStudent.Email;
                student.ClassName = updateStudent.ClassName;
            }
            return student;
        }
    }
}
