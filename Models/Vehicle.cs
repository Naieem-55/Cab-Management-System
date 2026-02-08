using System.ComponentModel.DataAnnotations;
using Cab_Management_System.Models.Enums;

namespace Cab_Management_System.Models
{
    public class Vehicle : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Make { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Model { get; set; } = string.Empty;

        [Required]
        [Range(1900, 2100)]
        public int Year { get; set; }

        [Required]
        [StringLength(30)]
        public string Color { get; set; } = string.Empty;

        [Required]
        [Range(1, 50)]
        public int Capacity { get; set; }

        [Required]
        [Display(Name = "Fuel Type")]
        public FuelType FuelType { get; set; }

        [Required]
        public VehicleStatus Status { get; set; } = VehicleStatus.Available;

        [Required]
        [Range(0, double.MaxValue)]
        public double Mileage { get; set; }

        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
        public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
