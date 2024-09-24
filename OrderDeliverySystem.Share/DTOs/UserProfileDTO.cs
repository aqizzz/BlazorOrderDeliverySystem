using OrderDeliverySystem.Share.Util;
using System.ComponentModel.DataAnnotations;
using static OrderDeliverySystem.Share.Data.ErrorMessages;
using static OrderDeliverySystem.Share.Data.Constants.User;

namespace OrderDeliverySystem.Share.DTOs
{
    public class UserProfileDTO : AddressUpdateDTO
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = RequiredAttributeErrorMessage)]
        [EmailAddress(ErrorMessage = InvalidEmailFormatErrorMessage)]
        [StringLength(MaxEmailLength, ErrorMessage = StringLengthErrorMessage, MinimumLength = MinEmailLength)]
        public string Email { get; set; } = string.Empty;

        [StringLength(MaxNameLength, ErrorMessage = StringLengthErrorMessage, MinimumLength = MinNameLength)]
        public string? FirstName { get; set; }

        [StringLength(MaxNameLength, ErrorMessage = StringLengthErrorMessage, MinimumLength = MinNameLength)]
        public string? LastName { get; set; }

        [PhoneFormat]
        public string? Phone { get; set; }
    }
}
