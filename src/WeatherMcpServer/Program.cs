using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;

using WeatherMcpServer.Extensions;
using WeatherMcpServer.Tools;

var builder = Host.CreateApplicationBuilder();

// Configure all logs to go to stderr (stdout is used for the MCP protocol messages).
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

// Serilog configuration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Configuration
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

builder.Services
    .RegisterServices()
    .AddLogging(loggingBuilder =>
    {
        loggingBuilder.ClearProviders();
        loggingBuilder.AddSerilog(dispose: true);
    })
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<RandomNumberTools>()
    .WithTools<WeatherTools>();


var host = builder.Build();


#if DEBUG
{
    var controller = host.Services.GetRequiredService<RandomNumberTools>();
    var random = controller.GetRandomNumber(40, 50);
    Console.WriteLine(random);
}
{
    var controller = host.Services.GetRequiredService<WeatherTools>();
    var weather = await controller.GetCurrentWeather("London", "UK");
    Console.WriteLine(weather);
}
#endif

await host.RunAsync();
