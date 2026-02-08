using System.ComponentModel.DataAnnotations;

namespace Cab_Management_System.Models
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

        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
