using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CabManagementSystem.Models.ViewModels
{
    public class ProfileViewModel
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;

        [Display(Name = "Two-Factor Authentication")]
        public bool TwoFactorEnabled { get; set; }

        [Display(Name = "Profile Photo")]
        public string? ProfilePhotoPath { get; set; }

        [Display(Name = "Profile Photo")]
        public IFormFile? ProfilePhoto { get; set; }
    }
}
