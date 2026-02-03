using System.ComponentModel.DataAnnotations;

namespace Cab_Management_System.Models
{
    public class Route : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Origin { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Destination { get; set; } = string.Empty;

        [Required]
        [Range(0.1, double.MaxValue)]
        [Display(Name = "Distance (km)")]
        public double Distance { get; set; }

        [Required]
        [Display(Name = "Estimated Time (hours)")]
        [Range(0.1, double.MaxValue)]
        public double EstimatedTimeHours { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue)]
        [Display(Name = "Base Cost")]
        public decimal BaseCost { get; set; }

        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
