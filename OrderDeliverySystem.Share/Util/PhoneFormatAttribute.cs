using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using static OrderDeliverySystem.Share.Data.Constants.User;
using static OrderDeliverySystem.Share.Data.ErrorMessages;

namespace OrderDeliverySystem.Share.Util
{
    public class PhoneFormatAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null or (object)"")
            {
                return ValidationResult.Success;
            }

            string phoneNumber = value.ToString();
            if (Regex.IsMatch(phoneNumber, PhoneNumberRegularExpression))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage ?? PhoneFormatDoesNotMatchErrorMessage);
        }
    }
}
