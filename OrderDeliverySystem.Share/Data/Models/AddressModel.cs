namespace OrderDeliverySystem.Share.Data.Models
{
    public class AddressModel
    {
        public int AddressId { get; set; }
        public int UserId { get; set; }
        public string? Type { get; set; }
        public string? Unit { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? Postcode { get; set; }
        public required User User { get; set; }
    }

}
