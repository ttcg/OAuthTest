using System.Collections.Generic;
using System.Security.Claims;

namespace OAuthTest.Students.Models
{
    public class DiagnosticViewModel
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public List<Claim> JwtClaims { get; set; }
        public List<Claim> UserClaims { get; set; }
    }
}
