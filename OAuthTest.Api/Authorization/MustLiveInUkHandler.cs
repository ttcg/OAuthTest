using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthTest.ApiStudents.Authorization
{

    public class MustLiveInUkHandler : AuthorizationHandler<MustLiveInUkRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MustLiveInUkRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == Constants.CustomClaimTypes.Country))
            {
                return Task.CompletedTask;
            }

            var country = context.User.FindFirst(c => c.Type == Constants.CustomClaimTypes.Country).Value;

            if (string.Equals(country, "UK", StringComparison.InvariantCultureIgnoreCase))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
