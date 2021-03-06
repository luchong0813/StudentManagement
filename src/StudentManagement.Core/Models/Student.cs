﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagement.Models
{
    /// <summary>
    /// 学生模型
    /// </summary>
    public class Student
    {
        public int Id { get; set; }
        [Display(Name = "姓名")]
        [Required(ErrorMessage = "请输入姓名"), MaxLength(10, ErrorMessage = "姓名不能超过10位字符")]
        public string Name { get; set; }
        [Display(Name = "邮箱")]
        [Required(ErrorMessage = "请选择一个班级")]
        public ClassName? ClassName { get; set; }

        [Display(Name = "邮箱")]
        [Required(ErrorMessage = "请输入邮箱"), RegularExpression(@"^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$", ErrorMessage = "邮箱格式不正确")]
        public string Email { get; set; }

        [Display(Name = "头像")]
        public string Photo { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }

        /// <summary>
        /// 入学时间
        /// </summary>
        public DateTime EnrollmentDate { get; set; }

        public ICollection<StudentCourse> StudentCourses { get; set; }
    }
}
