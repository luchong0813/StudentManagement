using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.CustomerMiddlewares.Utils
{
    public class ValidEmailDomainAttribute : ValidationAttribute
    {
        private readonly string allowedDomail;

        public ValidEmailDomainAttribute(string allowedDomain)
        {
            this.allowedDomail = allowedDomain;
        }

        public override bool IsValid(object value)
        {
            string[] strings = value.ToString().Split('@');
            return strings[1].ToUpper() == allowedDomail.ToUpper();
        }
    }
}
