using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cab_Management_System.Models.Enums;

namespace Cab_Management_System.Models
{
    public class Trip : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public int DriverId { get; set; }

        [ForeignKey("DriverId")]
        public Driver Driver { get; set; } = null!;

        [Required]
        public int VehicleId { get; set; }

        [ForeignKey("VehicleId")]
        public Vehicle Vehicle { get; set; } = null!;

        [Required]
        public int RouteId { get; set; }

        [ForeignKey("RouteId")]
        public Models.Route Route { get; set; } = null!;

        [Required]
        [StringLength(100)]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        [Display(Name = "Customer Phone")]
        public string CustomerPhone { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(100)]
        [Display(Name = "Customer Email")]
        public string? CustomerEmail { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Trip Date")]
        public DateTime TripDate { get; set; }

        [Required]
        public TripStatus Status { get; set; } = TripStatus.Pending;

        [Required]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue)]
        public decimal Cost { get; set; }

        public Billing? Billing { get; set; }
    }
}
