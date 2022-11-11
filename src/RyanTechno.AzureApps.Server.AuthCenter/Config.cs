using Duende.IdentityServer.Models;

namespace RyanTechno.AzureApps.Server.AuthCenter;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        { 
            new IdentityResources.OpenId()
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
            { 
                new ApiScope(name: "stock", displayName: "Stock Market Statistics"),
                new ApiScope(name: "exchange", displayName: "Exchange Rate Services"),
            };

    public static IEnumerable<Client> Clients =>
        new Client[] 
            { 
                new Client
                {
                    ClientId = "stock_mobile",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets = { new Secret("94TQ7n7f8E".Sha256()) },

                    AllowedScopes = { "stock" },
                },
                new Client
                {
                    ClientId = "exchange_api",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets = { new Secret("bpzh5bq0Cx".Sha256()) },

                    AllowedScopes = { "exchange" },
                }
            };
}