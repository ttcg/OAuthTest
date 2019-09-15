using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace OAuthTest.IDP
{
    public static class Config
    {
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "78452916-D260-4219-927C-954F4E987E70",
                    Username = "ttcg",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.GivenName, "TTC"),
                        new Claim(JwtClaimTypes.FamilyName, "G"),
                        new Claim(JwtClaimTypes.Address, "Penn Way"),
                        new Claim(JwtClaimTypes.Role, "Admin"),
                        new Claim(Constants.CustomClaimTypes.Country, "UK")
                    }
                },
                new TestUser
                {
                    SubjectId = "81E045ED-7EA0-4F13-BEE6-88459B3B27AB",
                    Username = "John",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.GivenName, "John"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Address, "Thiri Street"),
                        new Claim(JwtClaimTypes.Role, "NormalUser"),
                        new Claim(Constants.CustomClaimTypes.Country, "FR")
                    }
                },
                new TestUser
                {
                    SubjectId = "C333FA5C-78DC-4A29-BD76-185C2F22F717",
                    Username = "Alex",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.GivenName, "Alex"),
                        new Claim(JwtClaimTypes.FamilyName, "Webb"),
                        new Claim(JwtClaimTypes.Address, "Claremont Road"),
                        new Claim(Constants.CustomClaimTypes.Country, "MM")
                    }
                },
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResource("roles", "Your role(s)", new List<string> { JwtClaimTypes.Role }),
                new IdentityResource("country", "Your Residence Country", new List<string> { Constants.CustomClaimTypes.Country })
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {   
                new ApiResource(Constants.Clients.Api, "OAuth Test Api", new List<string> { JwtClaimTypes.Role, Constants.CustomClaimTypes.Country })
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientName = "OAuth Test Web",
                    ClientId = Constants.Clients.Students,
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AccessTokenLifetime = 60,
                    AllowOfflineAccess = true,
                    UpdateAccessTokenClaimsOnRefresh = true, // to reflect the latest changes in User Claims
                    RedirectUris = new List<string>
                    {
                        $"{Constants.Urls.StudentsUrl}/signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        $"{Constants.Urls.StudentsUrl}/signout-callback-oidc"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "roles",
                        "country",
                        Constants.Clients.Api
                    },                    
                    ClientSecrets =
                    {
                        new Secret(Constants.Secrets.SharedSecret.Sha256())
                    }
                },
                new Client
                {
                    ClientName = "Postman Test Client",
                    ClientId = "postman",
                    AllowedGrantTypes = GrantTypes.Code,
                    
                    RequireConsent = false,
                    RedirectUris = { "https://www.getpostman.com/oauth2/callback" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "roles",
                        "country",
                        Constants.Clients.Api
                    },
                    ClientSecrets =
                    {
                        new Secret(Constants.Secrets.SharedSecret.Sha256())
                    }
                }
            };
        }
    }

    
}
