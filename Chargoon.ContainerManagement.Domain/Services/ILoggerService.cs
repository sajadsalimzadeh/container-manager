using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Services
{
    public interface ILoggerService
    {
        void ClearBefore(DateTime datetime);
        void LogError(Exception ex, object args = null);
        void LogInformation(string message, object args = null);
    }
}
