using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer
{
    public static class Configuration
    {
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("ApiOne","Api 1")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client_id",
                    ClientSecrets = {new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials ,
                    AllowedScopes = { "ApiOne" }
                }
            };
        }
    }
}
