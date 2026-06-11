using System.ComponentModel.DataAnnotations;

namespace CabManagementSystem.Models.ViewModels
{
    public class ResendConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
    }
}
