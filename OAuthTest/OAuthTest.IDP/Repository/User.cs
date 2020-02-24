using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OAuthTest.IDP.Repository
{
    public class User
    {
        public string SubjectId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Surname { get; set; }
        public string FirstForename { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string UserId { get; set; }
        public string CompanyId { get; set; }
    }
}