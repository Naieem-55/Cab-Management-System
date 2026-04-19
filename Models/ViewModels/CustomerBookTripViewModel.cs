using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CabManagementSystem.Models.ViewModels
{
    public class CustomerBookTripViewModel
    {
        [Required(ErrorMessage = "Please select a route")]
        [Display(Name = "Route")]
        public int RouteId { get; set; }

        [Required(ErrorMessage = "Please select a trip date")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Trip Date")]
        public DateTime TripDate { get; set; } = DateTime.Now.AddDays(1);

        [StringLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Display(Name = "Points to Redeem")]
        [Range(0, int.MaxValue)]
        public int PointsToRedeem { get; set; } = 0;

        public int AvailablePoints { get; set; } = 0;

        public SelectList? AvailableRoutes { get; set; }
    }
}
