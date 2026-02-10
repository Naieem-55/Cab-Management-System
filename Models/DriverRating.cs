using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cab_Management_System.Models
{
    public class DriverRating : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public int TripId { get; set; }

        [ForeignKey("TripId")]
        public Trip Trip { get; set; } = null!;

        [Required]
        public int DriverId { get; set; }

        [ForeignKey("DriverId")]
        public Driver Driver { get; set; } = null!;

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; } = string.Empty;
    }
}
