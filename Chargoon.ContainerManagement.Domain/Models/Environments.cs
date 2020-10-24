using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chargoon.ContainerManagement.Domain.Models
{
    public class Environments
    {
        public string Type { get; set; }
        public string Branch { get; set; }
        public string DatabaseUsername { get; set; }
        public string DatabasePassword { get; set; }
        public string BuildVersion { get; set; }
        public string ContainerTag { get; set; }

        [JsonIgnore]
        public string ComposeProjectName { get; set; }
        [JsonIgnore]
        public string User { get; set; }
        [JsonIgnore]
        public string Registry { get; set; }
        [JsonIgnore]
        public int SqlServerPort { get; set; }
        [JsonIgnore]
        public int FileManagerPort { get; set; }
        [JsonIgnore]
        public int Didgah4Port { get; set; }
        [JsonIgnore]
        public int Didgah5Port { get; set; }
    }
}
