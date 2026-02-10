using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cab_Management_System.Models.ViewModels
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

        public SelectList? AvailableRoutes { get; set; }
    }
}
