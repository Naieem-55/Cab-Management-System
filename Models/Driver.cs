using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cab_Management_System.Models.Enums;

namespace Cab_Management_System.Models
{
    public class Driver : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [Display(Name = "License Number")]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "License Expiry")]
        public DateTime LicenseExpiry { get; set; }

        [Required]
        public DriverStatus Status { get; set; } = DriverStatus.Available;

        public ICollection<Trip> Trips { get; set; } = new List<Trip>();

        public ICollection<DriverRating> Ratings { get; set; } = new List<DriverRating>();
    }
}
