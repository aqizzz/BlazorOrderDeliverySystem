using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using static OrderDeliverySystem.Share.Data.Constants.Address;
using static OrderDeliverySystem.Share.Data.ErrorMessages;

namespace OrderDeliverySystem.Share.Util
{
    public class PostcodeFormatAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null or (object)"")
            {
                return ValidationResult.Success;
            }

            string postcode = value.ToString();
            if (Regex.IsMatch(postcode, PostcodeRegularExpression))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage ?? InvalidPostcodeFormatErrorMessage);
        }
    }
}
