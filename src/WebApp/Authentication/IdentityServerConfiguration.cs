using Duende.IdentityServer.Models;

namespace WebApp.Authentication
{
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
            new Client()
            {
                ClientId = "React",
                AllowedScopes = [ "Api" ],
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                [
                  new Secret("ChangeMe123")// TODO: Read from Secret manager
                ]
            }
        ];
    }
}
