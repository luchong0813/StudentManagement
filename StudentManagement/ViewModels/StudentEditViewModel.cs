using Microsoft.AspNetCore.Cors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.ViewModels
{
    public class StudentEditViewModel : StudentCreateViewModel
    {
        public string Id { get; set; }
        public string ExistPhontPath { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }
    }
}
