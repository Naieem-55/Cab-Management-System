using System.ComponentModel.DataAnnotations;
using Cab_Management_System.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cab_Management_System.Models.ViewModels
{
    public class TripViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Driver")]
        public int DriverId { get; set; }

        [Required]
        [Display(Name = "Vehicle")]
        public int VehicleId { get; set; }

        [Required]
        [Display(Name = "Route")]
        public int RouteId { get; set; }

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

        [Display(Name = "Customer")]
        public int? CustomerId { get; set; }

        public SelectList? AvailableDrivers { get; set; }
        public SelectList? AvailableVehicles { get; set; }
        public SelectList? AvailableRoutes { get; set; }
        public SelectList? AvailableCustomers { get; set; }
    }
}
