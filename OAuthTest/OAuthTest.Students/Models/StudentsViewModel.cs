using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthTest.Students.Models
{
    public class StudentsViewModel
    {
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string StreetAddress { get; set; }
        public string Role { get; set; }

        public List<string> Students { get; set; } = new List<string>();
    }
}
