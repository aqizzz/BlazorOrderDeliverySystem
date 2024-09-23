namespace OrderDeliverySystem.Share.Data.Models
{
    public class WorkerLocation
    {
        public int WorkerLocationId { get; set; } // Primary Key
        public int WorkerId { get; set; } // Foreign Key
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime? LastUpdated { get; set; }
        public required DeliveryWorker DeliveryWorker { get; set; }
    }
}
