using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace WebApp.Authentication
{
#if OpenIdConnect
    public class IdentityServerConfiguration
    {
        public static IEnumerable<ApiScope> Scopes =>
        [
            new ApiScope()
            {
                Name = "Api",
                DisplayName = "AiBot Api",
            }
        ];

        public static IEnumerable<Client> Clients =>
        [
            //new Client()
            //{
            //    ClientId = "React",
            //    AllowedScopes = [ "Api" ],
            //    AllowedGrantTypes = GrantTypes.Code, // Interactive?
            //    ClientSecrets =
            //    [
            //      new Secret("ChangeMe123")// TODO: Read from Secret manager
            //    ]
            //},
                 // interactive ASP.NET Core Web App
        new Client
        {
            ClientId = "web",
            ClientSecrets = { new Secret("secret".Sha256()) },

            AllowedGrantTypes = GrantTypes.Code,
            
            // where to redirect to after login
            RedirectUris = { "https://localhost:5002/signin-oidc" },

            // where to redirect to after logout
            PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile
            }
        }
        ];

        //Add support for the standard openid (subject id) and profile (first name, last name, etc) scope
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
    }
#endif
}
