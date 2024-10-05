using System.ComponentModel.DataAnnotations.Schema;

namespace OrderDeliverySystem.Share.Data.Models
{
    public class DeliveryWorker
    {
        public int WorkerId { get; set; }
        public int UserId { get; set; }
        public int IsAvailable { get; set; }

        [NotMapped]
        public bool WorkerAvailability
        {
            get => IsAvailable == 1;
            set => IsAvailable = value ? 1 : 0;
        }
        public decimal? CommissionRate { get; set; }
        public DateTime? LastTaskAssigned { get; set; }
        public required User User { get; set; }
        public WorkerLocation? WorkerLocation { get; set; }

        public ICollection<DeliveryTask>? DeliveryTasks { get; set; }

    }

}
