using OrderDeliverySystem.Share.Util;

namespace OrderDeliverySystem.Share.DTOs
{
    public class AddressCreateDTO : AddressUpdateDTO
    {
        public int AddressId { get; set; }
        public string? ContactName { get; set; }

        [PhoneFormat]
        public string? Phone { get; set; }

    }
}
