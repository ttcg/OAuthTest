using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
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

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
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
                }

                var claimsToReturn = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();

                context.IssuedClaims = claimsToReturn;
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();

            var user = _userRepository.FindBySubjectId(sub);

            context.IsActive = user != null;
        }
    }

}
