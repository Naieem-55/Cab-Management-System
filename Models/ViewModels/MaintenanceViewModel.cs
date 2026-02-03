using System.ComponentModel.DataAnnotations;
using Cab_Management_System.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cab_Management_System.Models.ViewModels
{
    public class MaintenanceViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Vehicle")]
        public int VehicleId { get; set; }

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

        public SelectList? AvailableVehicles { get; set; }
    }
}
