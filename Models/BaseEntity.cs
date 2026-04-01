using System.ComponentModel.DataAnnotations;

namespace CabManagementSystem.Models
{
    public abstract class BaseEntity
    {
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }
    }
}
