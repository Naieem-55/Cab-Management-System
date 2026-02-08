using System.ComponentModel.DataAnnotations;
using Cab_Management_System.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cab_Management_System.Models.ViewModels
{
    public class ExpenseViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Vehicle")]
        public int? VehicleId { get; set; }

        [Display(Name = "Trip")]
        public int? TripId { get; set; }

        [Required]
        public ExpenseCategory Category { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(100)]
        [Display(Name = "Approved By")]
        public string? ApprovedBy { get; set; }

        public SelectList? AvailableVehicles { get; set; }
        public SelectList? AvailableTrips { get; set; }
    }
}
