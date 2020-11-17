namespace Chargoon.ContainerManagement.Domain.Models
{
    public class AppSettings
    {
        public AppSettingsDocker Docker { get; set; } = new AppSettingsDocker();
        public AppSettingsIdentity Identity { get; set; } = new AppSettingsIdentity();
        public AppSettingsHangfire Hangfire { get; set; } = new AppSettingsHangfire();
        public AppSettingsLogging Logging { get; set; } = new AppSettingsLogging();
        public AppSettingsImage Image { get; set; } = new AppSettingsImage();
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

    public class AppSettingsHangfire
    {
        public string DockerSystemPruneCron { get; set; } = "*/10 * * * *";
        public string DockerClearExitedCommandCacheCron { get; set; } = "1 * * * *";
        public string ClearLogCron { get; set; } = "0 0 * * *";
		public string ClearImageBuildLogCron { get; set; } = "0 0 * * *";
    }

    public class AppSettingsIdentity
    {
        public string Secret { get; set; }
        public int Timeout { get; set; } = 30; //Minutes
    }

    public class AppSettingsImage
    {
        public int BuildLogLifetime { get; set; } = 7; //Day
    }

    public class AppSettingsLogging
    {
        public int Lifetime { get; set; } = 30; //Days
    }
}