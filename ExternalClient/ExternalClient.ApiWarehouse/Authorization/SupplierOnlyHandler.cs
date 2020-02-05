using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace ExternalClient.ApiWarehouse.Authorization
{

    public class SupplierOnlyHandler : AuthorizationHandler<SupplierOnlyRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SupplierOnlyRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == $"client_{Constants.CustomClaimTypes.SupplierId}"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
