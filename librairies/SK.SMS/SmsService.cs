using SK.Sms.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace SK.Sms
{
    public class SmsService : ISmsService
    {
        private readonly SmsSettings _smsSettings;
        private readonly ILogger<SmsService> _logger;

        public SmsService(
            [NotNull] IOptions<SmsSettings> smsSettings,
            [NotNull] ILogger<SmsService> logger
        )
        {
            _smsSettings = smsSettings.Value;
            _logger = logger;
        }

        public void Initialize()
        {
            try
            {
                _logger.LogInformation("Starting Twilio configuration.");
                TwilioClient.Init(_smsSettings.AccountSid, _smsSettings.AuthToken);
                _logger.LogInformation("Twilio Configuration has been done.");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while configuring Twilio.", ex);
            }
        }

        public async Task<bool> SendAsync(string body, string to)
        {
            try
            {
                var message = await SendAsync(body, to, _smsSettings.PhoneNumber);

                if (message == null
                    || message.Status == MessageResource.StatusEnum.Failed
                    || message.Status == MessageResource.StatusEnum.Undelivered)
                {
                    return false;
                }

                _logger.LogInformation($"[{nameof(SmsService)}] Sms has been sent. from {_smsSettings.PhoneNumber} to: {to} body: {body}.");

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"[{nameof(SmsService)}] Error sending Sms. from {_smsSettings.PhoneNumber} to: {to} body: {body}.", e);
                return false;
            }
        }

        public async Task<MessageResource> SendAsync(string body, string to, string from = null)
        {
            MessageResource message = null;

            try
            {
                if (from == null)
                {
                    from = _smsSettings.PhoneNumber;
                }

                message = await MessageResource.CreateAsync(
                    from: new Twilio.Types.PhoneNumber(from),
                    to: new Twilio.Types.PhoneNumber(to),
                    body: body
                );

                _logger.LogInformation($"[{nameof(SmsService)}] Sms has been sent. from: {from} to: {to} body: {body} Status: {message?.Status}.");

                return message;
            }
            catch (Exception e)
            {
                _logger.LogError($"[{nameof(SmsService)}] Error sending Sms. from: {from} to: {to} body: {body} Status: {message?.Status}.", e);
                return message;
            }
        }
    }
}
