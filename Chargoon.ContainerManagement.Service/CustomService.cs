using Chargoon.ContainerManagement.Domain.Dtos.Customs;
using Chargoon.ContainerManagement.Domain.Models;
using Chargoon.ContainerManagement.Domain.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Chargoon.ContainerManagement.Service
{
    public class CustomService : ICustomService
    {
        private readonly AppSettings appSettings;

        public CustomService(IOptions<AppSettings> options)
        {
            appSettings = options.Value;
        }
        private string GetDevelopementPackBranchDateDirectory(string branch, DateTime date)
        {
            return Path.Combine(appSettings.Custome.DevelopmentPackPath, branch, date.ToString("yyyy.MM.dd"));
        }
        public IEnumerable<CustomNightlyBuildLogDto> GetNightlyBuildLogs(string branch, DateTime date)
        {
            var path = GetDevelopementPackBranchDateDirectory(branch, date);
            if (!Directory.Exists(path)) return new List<CustomNightlyBuildLogDto>();
            var list = new List<CustomNightlyBuildLogDto>();
            list.Add(new CustomNightlyBuildLogDto() { Branch = branch, Date = date, Name = "1.DidgahShared" });
            list.Add(new CustomNightlyBuildLogDto() { Branch = branch, Date = date, Name = "2.Didgah4" });
            list.Add(new CustomNightlyBuildLogDto() { Branch = branch, Date = date, Name = "3.Didgah5" });
            list.Add(new CustomNightlyBuildLogDto() { Branch = branch, Date = date, Name = "4.DidgahWebAPI" });
            list.Add(new CustomNightlyBuildLogDto() { Branch = branch, Date = date, Name = "Databases.NightlyBuild" });
            return list;
        }

        public FileStream GetNightlyBuildLogStream(string branch, DateTime date, string filename)
        {
            var path = Path.Combine(GetDevelopementPackBranchDateDirectory(branch, date), filename + ".html");
            if (!File.Exists(path)) throw new FileNotFoundException();
            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }
    }
}
