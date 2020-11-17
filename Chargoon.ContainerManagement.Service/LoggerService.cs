using Chargoon.ContainerManagement.Domain.DataModels;
using Chargoon.ContainerManagement.Domain.Dtos.Users;
using Chargoon.ContainerManagement.Domain.Models;
using Chargoon.ContainerManagement.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Chargoon.ContainerManagement.Service
{
    public class LoggerService : ILoggerService
    {
        class ExtraInfo
        {
            public UserGetDto User { get; set; }
            public string Method { get; set; }
            public string Class { get; set; }
            public string Assembly { get; set; }
        }

        private readonly ILogger logger;
        private readonly AppSettings appSettings;
        private readonly IHttpContextAccessor httpContextAccessor;

        private UserGetDto User { get { return httpContextAccessor?.HttpContext?.Items["User"] as UserGetDto; } }

        public LoggerService(
            ILoggerProvider loggerProvider,
            IOptions<AppSettings> appSettings,
            IHttpContextAccessor httpContextAccessor)
        {
            this.logger = loggerProvider.CreateLogger(GetType().Name);
            this.appSettings = appSettings.Value;
            this.httpContextAccessor = httpContextAccessor;
        }

        private ExtraInfo GetExteraInfoValues()
        {
            var st = new StackTrace();
            StackFrame frame;
            MethodBase method;
            var i = 1;
            do
            {
                i++;
                frame = st.GetFrame(i);
                method = frame.GetMethod();
            } while (method?.DeclaringType == typeof(LoggerService));

            return new ExtraInfo()
            {
                User = User,
                Method = method?.Name,
                Class = method?.DeclaringType.FullName,
                Assembly = method?.DeclaringType.Assembly.FullName,
            };
        }

        private string GetExtraInfoMessage()
        {
            return "{@Assembly} {@Class} {@Method} {@User} {@Args}";
        }

        public void LogError(Exception ex, object Args = null)
        {
            var e = GetExteraInfoValues();
            logger.LogError(ex, ex.Message + GetExtraInfoMessage(), e.Assembly, e.Class, e.Method, e.User, Args);
        }

        public void LogInformation(string message, object Args = null)
        {
            var e = GetExteraInfoValues();
            logger.LogInformation(message + GetExtraInfoMessage(), e.Assembly, e.Class, e.Method, e.User, Args);
        }

        public void ClearBefore(DateTime datetime)
        {
            var compareDate = datetime.ToString("yyyyMMdd");
            var dir = new DirectoryInfo("C:\\ProgramData\\Seq\\Logs");
            foreach (var file in dir.GetFiles())
            {
                var dateName = file.Name.Substring(4);
                if (dateName.CompareTo(compareDate) < 0)
                {
                    File.Delete(file.FullName);
                }
            }
        }
    }
}
