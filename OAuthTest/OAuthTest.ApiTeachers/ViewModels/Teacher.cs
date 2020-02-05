using System;
using System.Collections.Generic;

namespace OAuthTest.ApiTeachers.ViewModels
{
    public class Teacher
    {
        public Guid Id { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public List<Guid> Students { get; set; } = new List<Guid>();
    }
}
