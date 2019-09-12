using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthTest.Api.Authorization
{
    public class MustLiveInUkRequirement : IAuthorizationRequirement
    {
        public MustLiveInUkRequirement()
        {
        }
    }
}
