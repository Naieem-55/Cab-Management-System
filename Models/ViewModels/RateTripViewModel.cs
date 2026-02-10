using System.ComponentModel.DataAnnotations;

namespace Cab_Management_System.Models.ViewModels
{
    public class RateTripViewModel
    {
        public int TripId { get; set; }

        [Display(Name = "Driver")]
        public string DriverName { get; set; } = string.Empty;

        [Display(Name = "Route")]
        public string RouteInfo { get; set; } = string.Empty;

        [Display(Name = "Trip Date")]
        public DateTime TripDate { get; set; }

        [Required(ErrorMessage = "Please select a rating.")]
        [Range(1, 5, ErrorMessage = "Please select a rating between 1 and 5 stars.")]
        public int Rating { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }
    }
}
