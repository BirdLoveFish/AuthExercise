using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer
{
    public static class Configuration
    {
        //get apis
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                //让api中接收到claim
                new ApiResource("ApiOne","Api 1"
                    ,new []{"rc.Big.Color","rc.Color","email", "role" }
                    ),
                new ApiResource("ApiTwo","Api 2"),
            };
        }

        /// <summary>
        /// get user info
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> GetIdentity()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Phone(),
                new IdentityResource
                {
                    Name = "rc.scope",
                    UserClaims = {"rc.Color"
                    ,"rc.Big.Color"
                    }
                },
                new IdentityResource
                {
                    Name = "roles",
                    UserClaims = {"role"}
                }
            };
        }

        /// <summary>
        /// get clients
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                //credentials client.no user
                new Client
                {
                    ClientId = "client_id",
                    ClientSecrets = {new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials ,
                    AllowedScopes = { "ApiOne" }
                },
                //mvc client. use code flow
                new Client
                {
                    ClientId = "client_id_mvc",
                    ClientSecrets = {new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code ,
                    AllowedScopes = { 
                        "ApiOne", 
                        "ApiTwo",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Phone,
                        "rc.scope",
                        "roles"
                    },
                    //登陆回调地址
                    RedirectUris = { "http://localhost:5003/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5003/signin-out" },
                    //进入用户同意页面
                    RequireConsent = false,
                    //refresh token
                    AllowOfflineAccess = true,
                    //将用户的claim赋值到Client的User和id_token中
                    AlwaysIncludeUserClaimsInIdToken = true,
                },
                //js client.no cookie.implicit
                new Client
                {
                    ClientId = "client_id_js",
                    ClientName = "JavaScript Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris =           { "http://localhost:5004/callback.html" },
                    AllowedCorsOrigins =     { "http://localhost:5004" },
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "ApiOne",
                    }
                }
            };
        }
    }
}
