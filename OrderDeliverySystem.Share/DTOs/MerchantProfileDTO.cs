using System.ComponentModel.DataAnnotations;
using static OrderDeliverySystem.Share.Data.ErrorMessages;
using static OrderDeliverySystem.Share.Data.Constants.Merchant;

namespace OrderDeliverySystem.Share.DTOs
{
    public class MerchantProfileDTO : CustomerProfileDTO
    {
        [Required(ErrorMessage = RequiredAttributeErrorMessage)]
        [StringLength(MaxBusinessNameLength, ErrorMessage = StringMaxLengthErrorMessage)]
        public required string BusinessName { get; set; }

        [StringLength(MaxPicLength, ErrorMessage = StringMaxLengthErrorMessage)]
        public string? MerchantPic { get; set; }
        public string? MerchantDescription { get; set; }
        public int? PreparingTime { get; set; }

    }
}
