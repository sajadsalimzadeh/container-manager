using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Services
{
    public interface IStartupService
    {
        void Run();
        void RunAsync();
    }
}
