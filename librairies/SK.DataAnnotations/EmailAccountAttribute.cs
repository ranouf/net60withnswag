using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SK.DataAnnotations
{
    [AttributeUsage(
        AttributeTargets.Property
        | AttributeTargets.Field
        | AttributeTargets.Parameter,
        AllowMultiple = false
    )]
    public sealed class EmailAccountAttribute : DataTypeAttribute
    {
        public EmailAccountAttribute() : base(DataType.EmailAddress)
        {
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            if (value is not string valueAsString)
            {
                return false;
            }

            return IsValidEmail(valueAsString);

            static bool IsValidEmail(string email)
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return false;
                }

                try
                {
                    return Regex.IsMatch(email,
                        @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                        RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
                }
                catch (RegexMatchTimeoutException)
                {
                    return false;
                }
            }
        }
    }
}
