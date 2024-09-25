using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderDeliverySystem.Share.Data.Models;

namespace OrderDeliverySystem.Share.DTOs
{
    public class WorkerDTO1
    {
        public int WorkerId { get; set; }
        public int UserId { get; set; }
        public bool WorkerAvailability { get; set; }
        public decimal? CommissionRate { get; set; }
        public DateTime? LastTaskAssigned { get; set; }
        public User User { get; set; }
        public WorkerLocation? WorkerLocation { get; set; }

        public ICollection<DeliveryTask>? DeliveryTasks { get; set; }
    }
}
