using System;
using System.Collections.Generic;
using System.Linq;

namespace OAuthTest.IDP.Repository
{
    public class UserRepository
    {
        private List<User> _users = new List<User>
        {
            new User()
            {
                SubjectId = "ABCDEF12-78DC-4A29-BD76-185C2F22F717",
                Username = "Tun Gwa",
                Email = "tungwa@gmail.com",
                FirstForename = "Tun",
                Surname = "Gwa",
                Country = "UK",
                Address = "Ealing London",
                UserId = "11112222",
                CompanyId = "33334444"
            },
            new User()
            {
                SubjectId = "78452916-D260-4219-927C-954F4E987E70",
                Username = "ttcg",
                Email = "ttcg2000@gmail.com",
                FirstForename = "TTC",
                Surname = "G",
                Country = "UK",
                Address = "Stevenage",
                UserId = "24022020",
                CompanyId = "999777"
            }
        };

        public User FindByEmail(string email)
        {
            return _users.SingleOrDefault(x => string.Equals(x.Email, email, StringComparison.InvariantCultureIgnoreCase));
        }

        public User FindByUsername(string username)
        {
            return _users.SingleOrDefault(x => string.Equals(x.Username, username, StringComparison.InvariantCultureIgnoreCase));
        }

        public User FindBySubjectId(string subjectId)
        {
            return _users.SingleOrDefault(x => string.Equals(x.SubjectId, subjectId, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
