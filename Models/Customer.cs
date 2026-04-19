using System.ComponentModel.DataAnnotations;

namespace CabManagementSystem.Models
{
    public class Customer : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Address { get; set; }

        [Display(Name = "Loyalty Points")]
        public int LoyaltyPoints { get; set; }

        public ICollection<Trip> Trips { get; set; } = new List<Trip>();

        public ICollection<TripFeedback> Feedbacks { get; set; } = new List<TripFeedback>();

        public ICollection<LoyaltyTransaction> LoyaltyTransactions { get; set; } = new List<LoyaltyTransaction>();
    }
}
