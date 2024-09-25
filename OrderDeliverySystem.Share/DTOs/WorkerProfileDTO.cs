using OrderDeliverySystem.Share.Util;

namespace OrderDeliverySystem.Share.DTOs
{
    public class WorkerProfileDTO : UserProfileDTO
    {
        public required bool WorkerAvailability { get; set; }

        [DecimalPrecision(4, 2)]
        public decimal? CommissionRate { get; set; }
        public DateTime? LastTaskAssigned { get; set; }

    }
}
