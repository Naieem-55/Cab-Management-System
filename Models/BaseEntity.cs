using System.ComponentModel.DataAnnotations;

namespace Cab_Management_System.Models
{
    public abstract class BaseEntity
    {
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }
    }
}
