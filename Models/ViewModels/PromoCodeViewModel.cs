using System.ComponentModel.DataAnnotations;
using CabManagementSystem.Models.Enums;

namespace CabManagementSystem.Models.ViewModels
{
    public class PromoCodeViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        [Display(Name = "Promo Code")]
        public string Code { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Discount Type")]
        public DiscountType DiscountType { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Discount value must be greater than zero.")]
        [Display(Name = "Discount Value")]
        public decimal DiscountValue { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Max Discount Amount")]
        public decimal? MaxDiscountAmount { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Minimum Trip Cost")]
        public decimal MinTripCost { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Usage limit must be at least 1.")]
        [Display(Name = "Usage Limit (blank = unlimited)")]
        public int? UsageLimit { get; set; }

        public int TimesUsed { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Valid From")]
        public DateTime ValidFrom { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Valid Until")]
        public DateTime ValidUntil { get; set; } = DateTime.Now.AddMonths(1);

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}
