using IdentityServer4.Models;
using IdentityServer4.Stores;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OAuthTest.IDP
{
    public class CustomClientStore : IClientStore
    {
        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            return await Task.Run(() => {
                return new Client
                {
                    ClientName = "Test Api Client",
                    ClientId = "TestApiClient",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    AllowedScopes = new List<string>
                        {
                            Constants.Clients.ApiStudents,
                            Constants.Clients.ApiTeachers
                        },

                    ClientSecrets =
                        {
                            new Secret("secret".Sha256())
                        }
                };
            });
        }
    }
}
