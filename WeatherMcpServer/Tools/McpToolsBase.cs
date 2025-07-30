using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Refit;

using System.Net;

namespace WeatherMcpServer.Tools;

public abstract class McpToolsBase(ILogger<WeatherTools> logger)
{
    protected async Task<string> ProcessRequest(Func<Task<string>> requestFunc)
    {
        try
        {
            return await requestFunc.Invoke();
        }
        catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
        {
            logger.LogWarning("Too much requests. Code 429.");
            return JsonConvert.SerializeObject(new ProblemDetails
            {
                Title = "Server Error",
                Detail = "Too many requests",
                Status = 429,
            });
        }
        catch (ApiException ex)
        {
            logger.LogError(ex, ex.Content);
            return JsonConvert.SerializeObject(new ProblemDetails
            {
                Title = "Server Error",
                Detail = ex.Message,
                Status = (int)ex.StatusCode,
                Errors = new Dictionary<string, string[]> { { "request", [ex.Content ?? ""] } }
            });
        }
    }
}