namespace OrderDeliverySystem.Share.DTOs
{
    public class UserProfileDTO : CustomerProfileDTO
    {
        public required string Role { get; set; }
    }
}
