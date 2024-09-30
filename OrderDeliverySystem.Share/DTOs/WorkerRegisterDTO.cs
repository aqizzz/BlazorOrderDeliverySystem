
using System.ComponentModel.DataAnnotations;
using static OrderDeliverySystem.Share.Data.ErrorMessages;
using static OrderDeliverySystem.Share.Data.Constants.User;

namespace OrderDeliverySystem.Share.DTOs
{
    public class WorkerRegisterDTO : WorkerProfileDTO
    {
        [Required(ErrorMessage = RequiredAttributeErrorMessage)]
        [DataType(DataType.Password)]
        [StringLength(MaxPasswordLength, ErrorMessage = StringLengthErrorMessage, MinimumLength = MinPasswordLength)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredAttributeErrorMessage)]
        [DataType(DataType.Password)]
        [StringLength(MaxPasswordLength, ErrorMessage = StringLengthErrorMessage, MinimumLength = MinPasswordLength)]
        [RegularExpression(PasswordRegularExpression,
            ErrorMessage = InvalidPasswordFormatErrorMessage)]
        [Compare(nameof(Password), ErrorMessage = PasswordsDoNotMatchErrorMessage)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
