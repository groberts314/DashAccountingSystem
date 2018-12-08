using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using DashAccountingSystem.Extensions;

namespace DashAccountingSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(ConfigureLogging)
                .UseSerilog()
                .CaptureStartupErrors(true);

        private static void ConfigureLogging(WebHostBuilderContext webHostBuilderContext, ILoggingBuilder loggingBuilder)
        {
            // Build the intermediate service provider
            var serviceProvider = loggingBuilder.Services.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();

            var setupLogger = loggerFactory.CreateLogger<Program>();

            Log.Logger = webHostBuilderContext.Configuration.ConfigureLogger(
                "dash-accounting",
                webHostBuilderContext.HostingEnvironment.EnvironmentName,
                out string message).CreateLogger();

            setupLogger.LogInformation(message);
        }
    }
}
