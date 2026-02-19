using System.ComponentModel.DataAnnotations;

namespace Cab_Management_System.Models
{
    public class Notification : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        [StringLength(200)]
        public string? Link { get; set; }

        public ApplicationUser? User { get; set; }
    }
}
