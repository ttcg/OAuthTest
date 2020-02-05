using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthTest.Students.Models
{
    public class AdminStudentsViewModel
    {
        public List<Student> Students { get; set; } = new List<Student>();

        public sealed class Student
        {
            public Guid Id { get; set; }
            public string Forename { get; set; }
            public string Surname { get; set; }
            public DateTime DateOfBirth { get; set; }
            public Guid ClassTeacherId { get; set; }
            public string TeacherName { get; set; }
        }
    }
}
