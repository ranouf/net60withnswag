namespace ApiWithAuthentication.Librairies.Common
{
    public static class Constants
    {
        public static class Roles
        {
            public const string Administrator = "Administrator";
            public const string Manager = "Manager";
            public const string User = "User";
        }

        public static class Project
        {
            public const string Name = "ApiWithAuthentication";
        }
        public static class Api
        {
            public static class V1
            {
                public static class Authentication
                {
                    public const string Url = "/api/v1/authentication";
                    public const string Register = Url + "/register";
                    public const string ConfirmRegistrationEmail = Url + "/confirmregistrationemail";
                    public const string ConfirmInvitationEmail = Url + "/confirminvitationemail";
                    public const string ResendEmailConfirmation = Url + "/resendemailconfirmation";
                    public const string ResetPassword = Url + "/resetpassword";
                    public const string PasswordForgotten = Url + "/passwordforgotten";
                    public const string Login = Url + "/login";
                }

                public static class Account
                {
                    public const string Url = "/api/v1/account/";
                    public const string Password = Url + "password";
                    public const string Profile = Url + "profile";
                }

                public static class AccountChild
                {
                    public const string Url = "/api/v1/account-child";
                    public const string Password = Url + "password";
                    public const string Profile = Url + "profile";
                }

                public static class User
                {
                    public const string Url = "/api/v1/user";
                    public const string Lock = Url + "/{id:guid}/lock";
                    public const string Unlock = Url + "/{id:guid}/unlock";
                }

                public static class Role
                {
                    public const string Url = "/api/v1/role";
                }
                public static class Items
                {
                    public const string Url = "/api/v1/items";
                }
            }
        }
    }
}