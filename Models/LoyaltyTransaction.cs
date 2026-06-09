using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CabManagementSystem.Models.Enums;

namespace CabManagementSystem.Models
{
    public class LoyaltyTransaction : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; } = null!;

        public int? TripId { get; set; }

        [ForeignKey("TripId")]
        public Trip? Trip { get; set; }

        [Required]
        [Display(Name = "Points")]
        public int Points { get; set; }

        [Required]
        [Display(Name = "Type")]
        public LoyaltyTransactionType Type { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;
    }
}
