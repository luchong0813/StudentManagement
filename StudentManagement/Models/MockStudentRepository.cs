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
                new Student{ Id=1,Name="张三",ClassName="17计应4班",Email="84512211@outlook.com"},
                new Student{ Id=2,Name="李四",ClassName="17计应5班",Email="451515jshjd@outlook.com"},
                new Student{ Id=3,Name="王五",ClassName="17计应4班",Email="sghdha52@qq.com"},
                new Student{ Id=4,Name="赵六",ClassName="17计应6班",Email="45xshxdjn22@outlook.com"},
                new Student{ Id=5,Name="鲁班",ClassName="17计应5班",Email="dxshjc1515251@163.com"}
            };
        }

        public IEnumerable<Student> GetAllStudents()
        {
            return _students;
        }

        public Student GetStudent(int id)
        {
            return _students.FirstOrDefault(s => s.Id == id);
        }
    }
}
