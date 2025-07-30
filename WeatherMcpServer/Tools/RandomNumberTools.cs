using System.ComponentModel;

using Microsoft.Extensions.Logging;

using ModelContextProtocol.Server;

/// <summary>
/// Sample MCP tools for demonstration purposes.
/// These tools can be invoked by MCP clients to perform various operations.
/// </summary>
internal class RandomNumberTools(ILogger<RandomNumberTools> logger)
{
    [McpServerTool]
    [Description("Generates a random number between the specified minimum and maximum values.")]
    public int GetRandomNumber(
        [Description("Minimum value (inclusive)")]
        int min = 0,
        [Description("Maximum value (exclusive)")]
        int max = 100)
    {
        var result = Random.Shared.Next(min, max);
        logger.LogDebug($"Random number {result} was generated.");
        return result;
    }
}