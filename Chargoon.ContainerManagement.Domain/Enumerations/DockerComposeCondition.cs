using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Enumerations
{
    public enum DockerComposeCondition
    {
        None = 0,
        Any = 1,
        OnFailure = 2,
    }
}
