using System;

namespace ExternalClient.Constants
{
    public class Urls
    {
        public const string IdentityServerProviderUrl = "https://localhost:44378";
        public const string ApiWarehouseurl = "https://localhost:44367";
    }

    public class Secrets
    {
        public const string SharedSecret = "8ZE7fDu4rcfHWYmK";       
    }

    public class Clients
    {
        public const string ApiWarehouse = "warehouseapi";
    }

    public class CustomClaimTypes
    {
        public const string RetailerId = "retailer";
        public const string SupplierId = "supplier";
    }

    public class TokenSettings
    {
        public const string ExpiresAt = "expires_at";
    }
}
