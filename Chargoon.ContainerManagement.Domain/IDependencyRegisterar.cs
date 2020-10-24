using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain
{
    public interface IDependencyRegisterar
    {
        void Run(IServiceCollection services);
    }
}
