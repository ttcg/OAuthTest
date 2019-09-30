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

        public List<string> Students { get; set; } = new List<string>();

        public List<string> Teachers { get; set; } = new List<string>();
    }
}
