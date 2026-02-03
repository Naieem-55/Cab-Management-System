using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cab_Management_System.Models.Enums;

namespace Cab_Management_System.Models
{
    public class MaintenanceRecord
    {
        public int Id { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [ForeignKey("VehicleId")]
        public Vehicle Vehicle { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue)]
        public decimal Cost { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Maintenance Date")]
        public DateTime Date { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Next Maintenance Date")]
        public DateTime? NextMaintenanceDate { get; set; }

        [Required]
        public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Scheduled;
    }
}
