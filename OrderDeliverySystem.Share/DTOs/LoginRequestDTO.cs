using System.ComponentModel.DataAnnotations;
using static OrderDeliverySystem.Share.Data.ErrorMessages;
using static OrderDeliverySystem.Share.Data.Constants.User;

namespace OrderDeliverySystem.Share.DTOs
{
    public class LoginRequestDTO
    {
        [Required(ErrorMessage = RequiredAttributeErrorMessage)]
        [EmailAddress(ErrorMessage = InvalidEmailFormatErrorMessage)]
        [StringLength(MaxEmailLength, ErrorMessage = StringLengthErrorMessage, MinimumLength = MinEmailLength)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredAttributeErrorMessage)]
        [DataType(DataType.Password)]
        [StringLength(MaxPasswordLength, ErrorMessage = StringLengthErrorMessage, MinimumLength = MinPasswordLength)]
        public string Password { get; set; } = string.Empty;
    }
}
