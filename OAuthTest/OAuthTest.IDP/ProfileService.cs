using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Newtonsoft.Json;
using OAuthTest.IDP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OAuthTest.IDP
{
    public class ProfileService : IProfileService
    {
        private readonly UserRepository _userRepository;
        public ProfileService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();

            if (context.RequestedClaimTypes.Any())
            {
                // Get data from Db               
                var user = _userRepository.FindBySubjectId(sub);

                var claims = new List<Claim>();

                if (user != null)
                {
                    claims.Add(new Claim("custom_user_id", user.UserId));
                    claims.Add(new Claim("custom_company_id", user.CompanyId));
                    claims.Add(new Claim(JwtClaimTypes.Address, user.Address));
                    claims.Add(new Claim("country", user.Country));
                    claims.Add(new Claim(JwtClaimTypes.Email, user.Email));
                    claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstForename));
                    claims.Add(new Claim(JwtClaimTypes.FamilyName, user.Surname));
                    claims.Add(new Claim("role", "Admin"));
                }

                if (context.Caller == IdentityServerConstants.ProfileDataCallers.UserInfoEndpoint)
                {
                    var bigSetOfClaims = new Dictionary<string, string>();

                    for (int i = 1; i<= 10; i++)
                    {
                        bigSetOfClaims.Add($"key{i}", Guid.NewGuid().ToString().ToSha256());
                    }

                    claims.Add(new Claim("permissions", JsonConvert.SerializeObject(bigSetOfClaims), IdentityServerConstants.ClaimValueTypes.Json));
                }

                context.AddRequestedClaims(claims);
                //context.IssuedClaims = claims;
            }

            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();

            var user = _userRepository.FindBySubjectId(sub);

            context.IsActive = user != null;

            return Task.CompletedTask;
        }
    }

}
