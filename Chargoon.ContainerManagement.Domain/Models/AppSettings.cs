namespace Chargoon.ContainerManagement.Domain.Models
{
    public class AppSettings
    {
        public AppSettingsDocker Docker { get; set; } = new AppSettingsDocker();
        public AppSettingsIdentity Identity { get; set; } = new AppSettingsIdentity();
        public AppSettingsHangfire Hangfire { get; set; } = new AppSettingsHangfire();
        public AppSettingsLogging Logging { get; set; } = new AppSettingsLogging();
        public AppSettingsImage Image { get; set; } = new AppSettingsImage();
        public AppSettingsCustome Custome { get; set; } = new AppSettingsCustome();
    }

    public class AppSettingsCustome
    {
        public string DevelopmentPackPath { get; set; }
    }

    public class AppSettingsDocker
    {
        public string Url { get; set; } = "http://localhost:2375";
        public bool SelfHostEnable { get; set; } = false;
		public int ImageCacheTimeout { get; set; } = 5; //Seconds
        public int ContainerCacheTimeout { get; set; } = 5; //Seconds
        public int ServiceCacheTimeout { get; set; } = 5; //Seconds
        public int NodeCacheTimeout { get; set; } = 5; //Seconds
        public string Repository { get; set; }
    }

    public class AppSettingsHangfire
    {
        public string DockerSystemPruneCron { get; set; } = "*/10 * * * *";
        public string DockerClearExitedCommandCacheCron { get; set; } = "1 * * * *";
        public string ClearLogCron { get; set; } = "0 0 * * *";
		public string ClearImageBuildLogCron { get; set; } = "0 0 * * *";
        public string RemoveExpiredTemplateCron { get; set; } = "0 0 * * *";
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