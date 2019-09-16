using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthTest.Teachers.Models
{
    public class PrivacyViewModel
    {
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string StreetAddress { get; set; }
        public string Role { get; set; }

        public List<string> Values { get; set; } = new List<string>();
    }
}
