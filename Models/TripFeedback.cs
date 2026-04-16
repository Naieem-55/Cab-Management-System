using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CabManagementSystem.Models.Enums;

namespace CabManagementSystem.Models
{
    public class TripFeedback : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public int TripId { get; set; }

        [ForeignKey("TripId")]
        public Trip Trip { get; set; } = null!;

        [Required]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; } = null!;

        [Required]
        [Range(1, 5, ErrorMessage = "Please select a rating between 1 and 5 stars.")]
        [Display(Name = "Overall Rating")]
        public int Rating { get; set; }

        [Required]
        [Display(Name = "Category")]
        public FeedbackCategory Category { get; set; }

        [Required]
        [StringLength(1000)]
        [Display(Name = "Feedback")]
        public string Comment { get; set; } = string.Empty;

        [Display(Name = "Status")]
        public FeedbackStatus Status { get; set; } = FeedbackStatus.Open;

        [StringLength(1000)]
        [Display(Name = "Admin Response")]
        public string? AdminResponse { get; set; }

        [StringLength(100)]
        [Display(Name = "Responded By")]
        public string? RespondedBy { get; set; }

        [Display(Name = "Response Date")]
        public DateTime? RespondedDate { get; set; }
    }
}
