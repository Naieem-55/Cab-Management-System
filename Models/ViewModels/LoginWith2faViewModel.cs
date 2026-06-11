using System.ComponentModel.DataAnnotations;

namespace CabManagementSystem.Models.ViewModels
{
    public class LoginWith2faViewModel
    {
        [Required]
        [StringLength(8, MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Verification Code")]
        public string TwoFactorCode { get; set; } = string.Empty;

        [Display(Name = "Remember this machine")]
        public bool RememberMachine { get; set; }

        public bool RememberMe { get; set; }
    }
}
