namespace Chargoon.ContainerManagement.Domain.Models
{
    public class AppSettings
    {
        public AppSettingsDocker Docker { get; set; }
        public AppSettingsIdentity Identity { get; set; }
        public AppSettingsHangfire Hangfire { get; set; }
        public AppSettingsLogging Logging { get; set; }
    }

    public class AppSettingsDocker
    {
        public string Url { get; set; } = "http://localhost:2375";
        public int ImageCacheTimeout { get; set; } = 10; //Seconds
        public int ContainerCacheTimeout { get; set; } = 10; //Seconds
        public int ServiceCacheTimeout { get; set; } = 10; //Seconds
        public int NodeCacheTimeout { get; set; } = 10; //Seconds
        public string Repository { get; set; }
    }

    public class AppSettingsDidgah
    {
        public string DevelopmentPackPath { get; set; }
        public string BuildLogPath { get; set; }
    }

    public class AppSettingsHangfire
    {
        public string DockerSystemPruneCron { get; set; } = "*/10 * * * *";
        public string DockerClearExitedCommandCacheCron { get; set; } = "1 * * * *";
        public string ClearLogCron { get; set; } = "1 * * * *";
        public string DidgahBuildCron { get; set; } = "0 5 * * *";
    }

    public class AppSettingsIdentity
    {
        public string Secret { get; set; }
        public int Timeout { get; set; } = 30; //Minutes
    }

    public class AppSettingsLogging
    {
        public int Lifetime { get; set; } = 30; //Days
    }
}