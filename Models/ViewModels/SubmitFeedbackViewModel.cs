using System.ComponentModel.DataAnnotations;
using CabManagementSystem.Models.Enums;

namespace CabManagementSystem.Models.ViewModels
{
    public class SubmitFeedbackViewModel
    {
        public int TripId { get; set; }

        [Display(Name = "Driver")]
        public string DriverName { get; set; } = string.Empty;

        [Display(Name = "Route")]
        public string RouteInfo { get; set; } = string.Empty;

        [Display(Name = "Trip Date")]
        public DateTime TripDate { get; set; }

        [Required(ErrorMessage = "Please select a rating between 1 and 5 stars.")]
        [Range(1, 5)]
        [Display(Name = "Overall Rating")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        [Display(Name = "Category")]
        public FeedbackCategory Category { get; set; }

        [Required(ErrorMessage = "Please provide your feedback.")]
        [StringLength(1000)]
        [Display(Name = "Your Feedback")]
        public string Comment { get; set; } = string.Empty;
    }
}
