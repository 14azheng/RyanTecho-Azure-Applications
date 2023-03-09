namespace RyanTechno.AzureApps.Common.Models;

public class AuthenticationInfo
{
    public string AcquireAccessTokenEndpoint { get; init; }

    public string ClientId { get; init; }

    public string ClientSecret { get; init; }

    public string Scope { get; init; }
}
