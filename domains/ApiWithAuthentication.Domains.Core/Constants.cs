namespace ApiWithAuthentication.Domains.Core
{
    public static class Constants
    {
        public static class Roles
        {
            public const string Administrator = "Administrator";
            public const string Manager = "Manager";
            public const string User = "User";
            public const string System = "System";
        }

        public static class Emails
        {
            public const string PasswordForgotten_Subject = "Password reset request";
            public const string PasswordForgotten_ResourceUrl = "Assets/EmailTemplates/PasswordForgotten.html";
            public const string RegisterUser_Subject = "Welcome {Firstname}, your account has been registered";
            public const string RegisterUser_ResourceUrl = "Assets/EmailTemplates/RegisterUser.html";
            public const string InviteUser_Subject = "Welcome {Firstname}, you have been invited";
            public const string InviteUser_ResourceUrl = "Assets/EmailTemplates/InviteUser.html";
            public const string ConfirmEmail_Subject = "Confirm Email";
            public const string ConfirmEmail_ResourceUrl = "Assets/EmailTemplates/ConfirmEmail.html";
        }
    }
}
