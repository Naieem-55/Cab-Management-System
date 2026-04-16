using System.ComponentModel.DataAnnotations;
using CabManagementSystem.Models.Enums;

namespace CabManagementSystem.Models.ViewModels
{
    public class FeedbackResponseViewModel
    {
        public int FeedbackId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string Comment { get; set; } = string.Empty;

        public int Rating { get; set; }

        public FeedbackCategory Category { get; set; }

        public FeedbackStatus CurrentStatus { get; set; }

        public string RouteInfo { get; set; } = string.Empty;

        public DateTime TripDate { get; set; }

        [Required(ErrorMessage = "Please provide a response.")]
        [StringLength(1000)]
        [Display(Name = "Response")]
        public string AdminResponse { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Update Status")]
        public FeedbackStatus NewStatus { get; set; }
    }
}
