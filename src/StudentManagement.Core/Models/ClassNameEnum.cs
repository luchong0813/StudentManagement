using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Models
{

    public enum ClassName
    {
        [Display(Name = "未分配")]
        None,
        [Display(Name = "17计应4班")]
        ClassFour,
        [Display(Name = "17计应5班")]
        ClassFive,
        [Display(Name = "17计应6班")]
        ClassSix
    }

}
