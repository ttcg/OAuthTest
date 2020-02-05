using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace ExternalClient.ApiWarehouse.Authorization
{

    public class RetailerOnlyHandler : AuthorizationHandler<RetailerOnlyRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RetailerOnlyRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == $"client_{Constants.CustomClaimTypes.RetailerId}"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
