namespace OrderDeliverySystem.Share.Data.Models
{
    public class RegisterModel
    {
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string Role { get; set; } = "customer";
    }
}