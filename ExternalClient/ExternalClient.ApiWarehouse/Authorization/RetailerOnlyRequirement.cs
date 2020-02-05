using Microsoft.AspNetCore.Authorization;

namespace ExternalClient.ApiWarehouse.Authorization
{
    public class RetailerOnlyRequirement : IAuthorizationRequirement
    {
        public RetailerOnlyRequirement()
        {
        }
    }
}
