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
            },
            new User()
            {
                SubjectId = "1",
                Username = "alicesmith",
                Email = "AliceSmith@email.com",
                FirstForename = "Alice",
                Surname = "Smith",
                Country = "UK",
                Address = "One Hacker Way",
                UserId = "19052020",
                CompanyId = "999777"
            },
            new User()
            {
                SubjectId = "C333FA5C-78DC-4A29-BD76-185C2F22F717",
                Username = "Alex",
                Email = "alex@gmail.com",
                FirstForename = "Alex",
                Surname = "Webb",
                Country = "MM",
                Address = "Claremont Road",
                UserId = "11112222",
                CompanyId = "33334444"
            },
            new User()
            {
                SubjectId = "81E045ED-7EA0-4F13-BEE6-88459B3B27AB",
                Username = "John",
                Email = "john@gmail.com",
                FirstForename = "John",
                Surname = "Smith",
                Country = "MM",
                Address = "Thiri Street",
                UserId = "11112222",
                CompanyId = "33334444"
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
