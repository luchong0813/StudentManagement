using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.ViewModels
{
    public class EmailAdressViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
