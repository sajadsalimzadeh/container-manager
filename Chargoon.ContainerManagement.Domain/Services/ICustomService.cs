using Chargoon.ContainerManagement.Domain.Dtos.Customs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Services
{
    public interface ICustomService
    {
        FileStream GetNightlyBuildLogStream(string branch, DateTime date, string filename);
        IEnumerable<CustomNightlyBuildLogDto> GetNightlyBuildLogs(string branch, DateTime date);
    }
}
