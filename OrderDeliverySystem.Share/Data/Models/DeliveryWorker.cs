namespace OrderDeliverySystem.Share.Data.Models
{
    public class DeliveryWorker
    {
        public int WorkerId { get; set; }
        public int UserId { get; set; }
        public bool WorkerAvailability { get; set; }
        public decimal? CommissionRate { get; set; }
        public DateTime? LastTaskAssigned { get; set; }
        public required User User { get; set; }
        public WorkerLocation? WorkerLocation { get; set; }

        public ICollection<DeliveryTask>? DeliveryTasks { get; set; }

    }

}
