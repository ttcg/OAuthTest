using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace ExternalClient.IDP
{
    public static class Config
    {

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource(Constants.Clients.ApiWarehouse, "Api Warehouse Resource", new List<string> { JwtClaimTypes.Role, Constants.CustomClaimTypes.RetailerId })
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            {
                var clients = new List<Client>();

                clients.AddRange(CreateExternalClients());

                return clients;
            }

            List<Client> CreateExternalClients()
            {
                return new List<Client>
                {
                    CreateRetailerClient(),
                    CreateSupplierClient()
                };

                Client CreateRetailerClient()
                {
                    return new Client
                    {
                        ClientName = "Test Retailer Client",
                        ClientId = "TestRetailerClient",

                        // no interactive user, use the clientid/secret for authentication
                        AllowedGrantTypes = GrantTypes.ClientCredentials,

                        AllowedScopes = new List<string>
                        {
                            Constants.Clients.ApiWarehouse
                        },

                        ClientSecrets =
                        {
                            new Secret("imaretailer".Sha256())
                        },

                        Claims = new List<Claim>
                        {
                        new Claim(Constants.CustomClaimTypes.RetailerId, "2E139221-3FE9-4E48-B1E8-B2A19E975E68")
                        }
                    };
                }

                Client CreateSupplierClient()
                {
                    return new Client
                    {
                        ClientName = "Test Supplier Client",
                        ClientId = "TestSupplierClient",

                        // no interactive user, use the clientid/secret for authentication
                        AllowedGrantTypes = GrantTypes.ClientCredentials,

                        AllowedScopes = new List<string>
                        {
                            Constants.Clients.ApiWarehouse
                        },

                        ClientSecrets =
                        {
                            new Secret("imasupplier".Sha256())
                        },

                        Claims = new List<Claim>
                        {
                        new Claim(Constants.CustomClaimTypes.SupplierId, "BF298D72-9172-4102-8701-F4DB9BE524F6")
                        }
                    };
                }
            }
        }
    }
}