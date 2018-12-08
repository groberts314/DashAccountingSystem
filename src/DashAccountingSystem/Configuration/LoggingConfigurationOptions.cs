namespace DashAccountingSystem.Configuration
{
    public class LoggingConfigurationOptions
    {
        public string LogDir { get; set; }
        public LogLevelConfiguration LogLevel { get; set; }
        public bool LogToConsole { get; set; }
        public bool LogToRollingFile { get; set; }
        public bool EnableMetricMonitoring { get; set; }
    }

    public class LogLevelConfiguration
    {
        public string Microsoft { get; set; }
        public string System { get; set; }
        public string Default { get; set; }

    }

    public static class LoggingConstants
    {
        public const string DefaultSource = "Default";
        public const string MicrosoftSource = "Microsoft";
        public const string SystemSource = "System";
    }
}
