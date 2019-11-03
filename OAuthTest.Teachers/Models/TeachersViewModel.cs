using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthTest.Teachers.Models
{
    public class TeachersViewModel
    {
        public List<string> Students { get; set; } = new List<string>();

        public List<Teacher> Teachers { get; set; } = new List<Teacher>();

        public sealed class Teacher
        {
            public Guid Id { get; set; }
            public string Forename { get; set; }
            public string Surname { get; set; }
            public List<Guid> Students { get; set; } = new List<Guid>();
        }
    }
}
