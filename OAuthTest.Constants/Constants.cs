﻿using System;

namespace OAuthTest.Constants
{
    public class Urls
    {
        public const string IdentityServerProviderUrl = "https://localhost:44378";
        public const string ApiStudentsUrl = "https://localhost:44367";
        public const string ApiTeachersUrl = "https://localhost:44331";
        public const string StudentsUrl = "https://localhost:44392";
        public const string TeachersUrl = "https://localhost:44344";
    }

    public class Secrets
    {
        public const string SharedSecret = "8ZE7fDu4rcfHWYmK";       
    }

    public class Clients
    {
        public const string Students = "oauthteststudents";
        public const string Teachers = "oauthtestteachers";
        public const string ApiStudents = "oauthtestapistudents";
        public const string ApiTeachers = "oauthtestapiteachers";
    }

    public class CustomClaimTypes
    {
        public const string Country = "country";
    }

    public class TokenSettings
    {
        public const string ExpiresAt = "expires_at";
    }

    public enum ApiTypes
    {
        Student,
        Teacher
    }
}
