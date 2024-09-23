using OrderDeliverySystem.Share.Util;
using System.ComponentModel.DataAnnotations;
using static OrderDeliverySystem.Share.Data.ErrorMessages;
using static OrderDeliverySystem.Share.Data.Constants.User;

namespace OrderDeliverySystem.Share.DTOs
{
    public class UserUpdateDTO
    {
        [Required(ErrorMessage = RequiredAttributeErrorMessage)]
        [EmailAddress(ErrorMessage = InvalidEmailFormatErrorMessage)]
        public string Email { get; set; }

        [StringLength(MaxNameLength, ErrorMessage = StringLengthErrorMessage, MinimumLength = MinNameLength)]
        public string? FirstName { get; set; }

        [StringLength(MaxNameLength, ErrorMessage = StringLengthErrorMessage, MinimumLength = MinNameLength)]
        public string? LastName { get; set; }

        [PhoneFormat(ErrorMessage = PhoneFormatDoesNotMatchErrorMessage)]
        public string? Phone { get; set; }
    }
}
