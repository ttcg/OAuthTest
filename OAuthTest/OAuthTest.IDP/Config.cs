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
                        new Claim(JwtClaimTypes.Email, "ttcg2000@gmail.com"),
                        new Claim(Constants.CustomClaimTypes.Country, "UK"),
                        new Claim("custom_user_id", "24022020"),
                        new Claim("custom_company_id", "999777"),
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
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Address(),
                new IdentityResource("roles", "Your role(s)", new List<string> { JwtClaimTypes.Role }),
                new IdentityResource("country", "Your Residence Country", new List<string> { Constants.CustomClaimTypes.Country }),
                new IdentityResource("custom_ids", "Custom Ids", new List<string> { "custom_user_id", "custom_company_id", "permissions"})
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource(Constants.Clients.ApiStudents, "OAuth Test Api Students", new List<string> { JwtClaimTypes.Role, Constants.CustomClaimTypes.Country, "custom_user_id" })
                {
                    ApiSecrets = { new Secret(Constants.Secrets.SharedSecret.ToSha256()) },
                    Scopes = { Constants.Clients.ApiStudents }
                },
                new ApiResource(Constants.Clients.ApiTeachers, "OAuth Test Api Teachers", new List<string> { JwtClaimTypes.Role, Constants.CustomClaimTypes.Country })
                {
                    ApiSecrets = { new Secret(Constants.Secrets.SharedSecret.ToSha256()) },
                    Scopes = { Constants.Clients.ApiTeachers }
                },
                new ApiResource(Constants.Clients.ApiAll, "OAuth Test Api All", new List<string> { JwtClaimTypes.Role, Constants.CustomClaimTypes.Country, "custom_user_id" })
                {
                    ApiSecrets = { new Secret(Constants.Secrets.SharedSecret.ToSha256()) },
                    Scopes = { Constants.Clients.ApiStudents, Constants.Clients.Teachers }
                },
                new ApiResource(Constants.Clients.ApiCourses, "OAuth Test Api Courses", new List<string> { JwtClaimTypes.Role, Constants.CustomClaimTypes.Country })
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes() =>
            new List<ApiScope>
            {
                new ApiScope(Constants.Clients.ApiStudents, "OAuth Test Api Students", new List<string> { JwtClaimTypes.Role, Constants.CustomClaimTypes.Country, "custom_user_id" }),
                new ApiScope(Constants.Clients.ApiTeachers, "OAuth Test Api Teachers", new List<string> { JwtClaimTypes.Role, Constants.CustomClaimTypes.Country }),
                new ApiScope(Constants.Clients.ApiCourses, "OAuth Test Api Courses", new List<string> { JwtClaimTypes.Role, Constants.CustomClaimTypes.Country })
            };

        public static IEnumerable<Client> GetClients()
        {
            {
                return new List<Client>
                {
                    CreateStudentsClient(),
                    CreateTeachersClient(),
                    CreatePostmanClient(),
                    CreateTestApiClient(),
                    CreateEnrolmentsClient(),
                    CreateTestReactClient(),
                    CreateResourceOwnerClient()
                };
            }

            Client CreateStudentsClient()
            {
                return new Client
                {
                    ClientName = "OAuth Test Students",
                    ClientId = Constants.Clients.Students,
                    AllowedGrantTypes = GrantTypes.Code,
                    AccessTokenLifetime = 300,
                    AllowOfflineAccess = true,
                    RequireConsent = false,

                    UpdateAccessTokenClaimsOnRefresh = true, // to reflect the latest changes in User Claims
                    AccessTokenType = AccessTokenType.Reference,
                    RedirectUris = new List<string>
                        {
                            $"{Constants.Urls.StudentsUrl}/signin-oidc"
                        },
                    PostLogoutRedirectUris = new List<string>
                        {
                            $"{Constants.Urls.StudentsUrl}/signout-callback-oidc"
                        },
                    FrontChannelLogoutUri = $"{Constants.Urls.StudentsUrl}/Home/Logout",
                    AllowedScopes = PopulateScopes(),
                    ClientSecrets =
                        {
                            new Secret(Constants.Secrets.SharedSecret.Sha256())
                        }
                };
            }

            Client CreateTeachersClient()
            {
                return new Client
                {
                    ClientName = "OAuth Test Teachers",
                    ClientId = Constants.Clients.Teachers,
                    AllowedGrantTypes = GrantTypes.Code,
                    AccessTokenLifetime = 300,
                    AllowOfflineAccess = true,
                    RequireConsent = false,
                    AccessTokenType = AccessTokenType.Reference,
                    UpdateAccessTokenClaimsOnRefresh = true, // to reflect the latest changes in User Claims
                    RedirectUris = new List<string>
                        {
                            $"{Constants.Urls.TeachersUrl}/signin-oidc"
                        },
                    PostLogoutRedirectUris = new List<string>
                        {
                            $"{Constants.Urls.TeachersUrl}/signout-callback-oidc"
                        },
                    FrontChannelLogoutUri = $"{Constants.Urls.TeachersUrl}/Home/Logout",
                    AllowedScopes = PopulateScopes(),
                    ClientSecrets =
                        {
                            new Secret(Constants.Secrets.SharedSecret.Sha256())
                        }
                };
            }

            Client CreatePostmanClient()
            {
                return new Client
                {
                    ClientName = "Postman Test Client",
                    ClientId = "postman",
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { "https://www.getpostman.com/oauth2/callback", "https://oauth.pstmn.io/v1/callback" },

                    RequirePkce = false,
                    AccessTokenType = AccessTokenType.Reference,
                    AllowOfflineAccess = true,
                    RequireConsent = false,
                    AllowedScopes = PopulateScopes(),
                    ClientSecrets =
                        {
                            new Secret(Constants.Secrets.SharedSecret.Sha256())
                        }
                };
            }

            Client CreateTestApiClient()
            {
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
            }

            Client CreateEnrolmentsClient()
            {
                var clientUrl = Constants.Urls.EnrolmentsUrl;
                return new Client
                {
                    ClientName = "Enrolment Web Application",
                    ClientId = Constants.Clients.Enrolments,
                    AllowedGrantTypes = GrantTypes.Code,
                    AccessTokenLifetime = 300,
                    AllowOfflineAccess = true,
                    RequireConsent = false,
                    UpdateAccessTokenClaimsOnRefresh = true, // to reflect the latest changes in User Claims
                    RedirectUris = new List<string>
                    {
                        $"{clientUrl}"
                    },
                    FrontChannelLogoutUri = $"{clientUrl}/Home/SignOut",
                    AllowedScopes = PopulateScopes(),
                    ClientSecrets =
                        {
                            new Secret(Constants.Secrets.SharedSecret.Sha256())
                        },
                    AllowPlainTextPkce = false,
                    RequirePkce = true
                };
            }

            Client CreateTestReactClient()
            {
                var clientUrl = "http://localhost:3000";
                return new Client
                {
                    ClientId = "react-test-client",
                    ClientName = "React Test Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    RequireConsent = false,

                    AccessTokenLifetime = 120,
                    AccessTokenType = AccessTokenType.Reference,

                    // where to redirect to after login
                    RedirectUris = { $"{clientUrl}/callback", $"{clientUrl}/silent.html" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { $"{clientUrl}/index.html" },

                    AllowedCorsOrigins = { "http://localhost:3000" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        Constants.Clients.ApiStudents
                    }

                };
            }

            Client CreateResourceOwnerClient()
            {
                return new Client
                {
                    ClientName = "Resource Owner Client",
                    ClientId = "resourceOwnerClient",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    AccessTokenType = AccessTokenType.Reference,
                    AllowOfflineAccess = true,
                    RequireConsent = false,
                    AllowedScopes = PopulateScopes(),
                    ClientSecrets =
                        {
                            new Secret(Constants.Secrets.SharedSecret.Sha256())
                        }
                };
            }

            ICollection<string> PopulateScopes() =>
                new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            IdentityServerConstants.StandardScopes.Email,
                            IdentityServerConstants.StandardScopes.Address,
                            IdentityServerConstants.StandardScopes.OfflineAccess,
                            "roles",
                            "country",
                            Constants.Clients.ApiStudents,
                            Constants.Clients.ApiTeachers,
                            Constants.Clients.ApiCourses,
                            Constants.Clients.ApiAll,
                            "custom_ids"
                };
        }
    }
}