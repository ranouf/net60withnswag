using SK.Extensions;
using SK.Session;
using SK.Smtp;
using ApiWithAuthentication.Domains.Core.Identity.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ApiWithAuthentication.Domains.Core.Emails
{
    public class EmailManager : IEmailManager
    {
        private readonly IUserSession _session;
        private readonly ISmtpService _smtpService;

        public EmailManager(
            IUserSession session,
            ISmtpService smtpService
        )
        {
            _session = session;
            _smtpService = smtpService;
        }

        public async Task SendConfirmEmailAsync(User user, string token)
        {
            var confirmEmailUrl = new Uri(_session.BaseUrl)
                .Append("/authentication")
                .Append("/confirmemail")
                .AddQueryStringParameter("token", token)
                .AddQueryStringParameter("email", user.Email);

            await SendEmailAsync(
                user.Email,
                Constants.Emails.ConfirmEmail_Subject,
                Constants.Emails.ConfirmEmail_ResourceUrl,
                new Dictionary<string, string>
                {
                    { "baseUrl", _session.BaseUrl },
                    { "firstname", user.Firstname },
                    { "lastname", user.Lastname },
                    { "confirmEmailUrl", confirmEmailUrl.ToString() },
                }
            );
        }

        public async Task SendPasswordForgottenEmailAsync(User user, string token)
        {
            var resetPasswordUrl = new Uri(_session.BaseUrl)
                .Append("/authentication")
                .Append("/resetpassword")
                .AddQueryStringParameter("token", token)
                .AddQueryStringParameter("email", user.Email);

            await SendEmailAsync(
                user.Email,
                Constants.Emails.PasswordForgotten_Subject,
                Constants.Emails.PasswordForgotten_ResourceUrl,
                new Dictionary<string, string>
                {
                    { "firstname", user.Firstname },
                    { "lastname", user.Lastname },
                    { "resetPasswordUrl", resetPasswordUrl.ToString() },
                }
            );
        }

        public async Task SendInviteUserEmailAsync(User user, string token)
        {
            var invitationUrl = new Uri(_session.BaseUrl)
                .Append("/authentication")
                .Append("/invite")
                .AddQueryStringParameter("token", token)
                .AddQueryStringParameter("email", user.Email);

            await SendEmailAsync(
                user.Email,
                Constants.Emails.InviteUser_Subject,
                Constants.Emails.InviteUser_ResourceUrl,
                new Dictionary<string, string>
                {
                    { "invitationUrl", invitationUrl.ToString() },
                    { "firstname", user.Firstname },
                    { "lastname", user.Lastname },
                    { "createdBy.fullName", user.CreatedByUser.FullName },
                }
            );
        }

        #region Private
        private async Task SendEmailAsync(
            string to,
            string subject,
            string resourceUrl,
            Dictionary<string, string> dictionary
        )
        {
            var stream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, resourceUrl));
            using var reader = new StreamReader(stream);
            var body = reader.ReadToEnd();
            foreach (var item in dictionary)
            {
                subject = subject.Replace($"{{{item.Key}}}", item.Value);
                body = body.Replace($"{{{item.Key}}}", item.Value);
            }
            await _smtpService.SendEmailAsync(to, subject, body);
        }
        #endregion
    }
}
