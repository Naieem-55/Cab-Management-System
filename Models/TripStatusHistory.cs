using CabManagementSystem.Models.Enums;

namespace CabManagementSystem.Models
{
    public class TripStatusHistory
    {
        public int Id { get; set; }

        public int TripId { get; set; }
        public Trip? Trip { get; set; }

        public TripStatus Status { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.Now;
    }
}
