using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderDeliverySystem.Share.Data
{
    internal class ErrorMessages
    {
        public const string StringLengthErrorMessage
            = "The {0} must be at least {2} characters long.";

        public const string PasswordsDoNotMatchErrorMessage
            = "The password and confirmation password do not match.";

        public const string PhoneFormatDoesNotMatchErrorMessage
            ="Phone number must be in the format xxx-xxx-xxxx";

        public const string RequiredAttributeErrorMessage
            = "{0} is required";

        public const string InvalidEmailFormatErrorMessage
            = "Invalid email address";

        public const string InvalidPasswordFormatErrorMessage
            = "Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character";
    }
}
