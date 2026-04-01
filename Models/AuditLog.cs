using System.ComponentModel.DataAnnotations;

namespace CabManagementSystem.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string EntityName { get; set; } = string.Empty;

        public int EntityId { get; set; }

        [Required]
        [StringLength(20)]
        public string Action { get; set; } = string.Empty; // Create, Update, Delete

        public string? Changes { get; set; } // JSON

        [StringLength(450)]
        public string? UserId { get; set; }

        [StringLength(100)]
        public string? UserName { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
