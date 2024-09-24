using System.ComponentModel.DataAnnotations;
using static OrderDeliverySystem.Share.Data.ErrorMessages;
using static OrderDeliverySystem.Share.Data.Constants.Address;

namespace OrderDeliverySystem.Share.DTOs
{
    public class AddressUpdateDTO
    {
        [Required(ErrorMessage = RequiredAttributeErrorMessage)]
        public string? Type { get; set; }

        [StringLength(MaxUnitLength, ErrorMessage = StringMaxLengthErrorMessage)]
        public string? Unit { get; set; }

        [StringLength(MaxAddressLength, ErrorMessage = StringMaxLengthErrorMessage)]
        public string? Address { get; set; }

        [StringLength(MaxCityLength, ErrorMessage = StringMaxLengthErrorMessage)]
        public string? City { get; set; }

        [StringLength(MaxProvinceLength, ErrorMessage = StringMaxLengthErrorMessage)]
        public string? Province { get; set; }

        [StringLength(MaxPostalCodeLength, ErrorMessage = StringMaxLengthErrorMessage)]
        public string? Postcode { get; set; }
    }
}
