using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer4.Events;
using IdentityServer4.Quickstart.UI;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OAuthTest.IDP.Repository;

namespace OAuthTest.IDP.Quickstart.Account
{
    public class ToolsController : Controller
    {
        private readonly IEventService _events;
        private readonly UserRepository _userRepository;
        private const string OriginalUserIdClaimName = "original_user_id";

        public ToolsController(IEventService events, UserRepository userRepository)
        {
            _events = events;
            _userRepository = userRepository;
        }

        public string Index()
        {
            return "Tools/Index";
        }

        [Authorize]
        public async Task<IActionResult> Impersonate(string userId, string returnUrl)
        {
            // Get user to impersonate from database
            var user = _userRepository.FindBySubjectId(userId);
            if (user == null)
                return Unauthorized("User not found");

            var currentUserId = User.FindFirst(JwtClaimTypes.Subject).Value.ToLower().ToString();

            // sign out current user
            await CustomSignOutAsync(currentUserId);

            // sign in as user to impersonate
            var additionalLocalClaims = new List<Claim>()
            {
                new Claim(OriginalUserIdClaimName, currentUserId) // store in the local claim to un-impersonate in the future
            };            
            await CustomSignInAsync(user, additionalLocalClaims);

            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect("~/");
            }
            else
            {
                return Redirect(returnUrl);
            }
        }

        [Authorize]
        public async Task<IActionResult> UnImpersonate(string returnUrl)
        {
            // get original user id back to Signin again
            var originalUserId = User.FindFirst(OriginalUserIdClaimName);
            if (originalUserId == null)
                return Unauthorized("There is no user found to unimpersonate");

            // Get user from database
            var user = _userRepository.FindBySubjectId(originalUserId.Value);
            if (user == null)
                return Unauthorized("User Id not found");

            // sign out current user
            var currentUserId = User.FindFirst(JwtClaimTypes.Subject).Value.ToLower().ToString();
            await CustomSignOutAsync(currentUserId);

            // sign in as original user
            await CustomSignInAsync(user, new List<Claim>());

            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect("~/");
            }
            else
            {
                return Redirect(returnUrl);
            }
        }

        private async Task CustomSignInAsync(User user, List<Claim> additionalLocalClaims)
        {
            await _events.RaiseAsync(new UserLoginSuccessEvent(user.Username, user.SubjectId, user.Username));

            // issue authentication cookie for user
            await HttpContext.SignInAsync(user.SubjectId, user.Username, additionalLocalClaims.ToArray());
        }

        private async Task CustomSignOutAsync(string currentUserId)
        {
            // delete local authentication cookie
            await HttpContext.SignOutAsync();

            // raise the logout event
            await _events.RaiseAsync(new UserLogoutSuccessEvent(currentUserId, string.Empty));
        }
    }
}