using Microsoft.AspNetCore.Authorization;

namespace ExternalClient.ApiWarehouse.Authorization
{
    public class SupplierOnlyRequirement : IAuthorizationRequirement
    {
        public SupplierOnlyRequirement()
        {
        }
    }
}
