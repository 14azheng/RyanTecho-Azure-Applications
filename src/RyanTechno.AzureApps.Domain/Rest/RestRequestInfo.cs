namespace RyanTechno.AzureApps.Domain.Rest;

public class RestRequestInfo
{
    public string RequestEndpoint { get; init; }

    public Dictionary<string, string> RequestHeaders { get; init; } = new();
}
