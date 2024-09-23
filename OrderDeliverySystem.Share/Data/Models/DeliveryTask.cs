namespace OrderDeliverySystem.Share.Data.Models
{
    public class DeliveryTask
    {
        public int TaskId { get; set; }
        public int WorkerId { get; set; }
        public int OrderId { get; set; }
        public DateTime? AssignedTime { get; set; }
        public DateTime? CompletedTime { get; set; }
        public required string Status { get; set; }
        public DeliveryWorker? DeliveryWorker { get; set; }
        public required Order Order { get; set; }
    }
}
