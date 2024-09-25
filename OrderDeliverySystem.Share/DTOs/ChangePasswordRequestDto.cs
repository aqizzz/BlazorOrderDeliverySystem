using System.ComponentModel.DataAnnotations;
using static OrderDeliverySystem.Share.Data.Constants.User;
using static OrderDeliverySystem.Share.Data.ErrorMessages;

namespace OrderDeliverySystem.Share.DTOs
{
    public class ChangePasswordRequestDto
    {
        [Required(ErrorMessage = RequiredAttributeErrorMessage)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = RequiredAttributeErrorMessage)]
        [StringLength(MaxPasswordLength, ErrorMessage = StringLengthErrorMessage, MinimumLength = MinPasswordLength)]
        [DataType(DataType.Password)]
        [RegularExpression(PasswordRegularExpression)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = RequiredAttributeErrorMessage)]
        [DataType(DataType.Password)]
        [StringLength(MaxPasswordLength, ErrorMessage = StringLengthErrorMessage, MinimumLength = MinPasswordLength)]
        [RegularExpression(PasswordRegularExpression,
            ErrorMessage = InvalidPasswordFormatErrorMessage)]
        [Compare(nameof(NewPassword), ErrorMessage = PasswordsDoNotMatchErrorMessage)]
        public string ConfirmNewPassword { get; set; } = string.Empty;

    }
}
