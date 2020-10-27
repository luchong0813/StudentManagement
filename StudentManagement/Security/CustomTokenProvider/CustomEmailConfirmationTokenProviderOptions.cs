using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Security
{
    /// <summary>
    /// 自定义邮箱验证令牌有效期
    /// </summary>
    public class CustomEmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
    {
    }
}
