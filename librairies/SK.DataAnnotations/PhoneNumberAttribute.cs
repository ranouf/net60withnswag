using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SK.DataAnnotations
{
    public sealed class PhoneNumberAttribute : DataTypeAttribute
    {
        public PhoneNumberAttribute() : base(DataType.PhoneNumber)
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

            return IsValidPhoneNumber(valueAsString);

            static bool IsValidPhoneNumber(string phoneNumber)
            {
                if (string.IsNullOrWhiteSpace(phoneNumber)) { 
                    return false; 
                }

                try
                {
                    return Regex.IsMatch(phoneNumber,
                        @"^\d{3}-\d{3}-\d{4}$");
                }
                catch (RegexMatchTimeoutException)
                {
                    return false;
                }
            }
        }
    }
}
