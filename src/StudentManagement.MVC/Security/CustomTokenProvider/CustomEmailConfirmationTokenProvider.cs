using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Security.CustomTokenProvider
{
    /// <summary>
    /// 自定义邮箱验证令牌提供程序
    /// </summary>
    /// <typeparam name="Tuser"></typeparam>
    public class CustomEmailConfirmationTokenProvider<Tuser> : DataProtectorTokenProvider<Tuser> where Tuser : class
    {
        public CustomEmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider, IOptions<CustomEmailConfirmationTokenProviderOptions> options, ILogger<DataProtectorTokenProvider<Tuser>> logger) : base(dataProtectionProvider, options, logger)
        {

        }
    }
}
