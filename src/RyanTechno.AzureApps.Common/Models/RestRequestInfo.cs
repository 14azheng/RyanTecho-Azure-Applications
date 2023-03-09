namespace RyanTechno.AzureApps.Common.Models;

public class RestRequestInfo
{
    public string RequestEndpoint { get; init; }

    public Dictionary<string, string> RequestHeaders { get; init; } = new();
}
