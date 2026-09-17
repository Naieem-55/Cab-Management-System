using System.ComponentModel.DataAnnotations;
using CabManagementSystem.Models.Enums;

namespace CabManagementSystem.Models.ViewModels
{
    public class PricingRuleViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Start Time")]
        public TimeSpan StartTime { get; set; } = new(8, 0, 0);

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "End Time")]
        public TimeSpan EndTime { get; set; } = new(10, 0, 0);

        // Empty by default: unchecked boxes post nothing, so a non-empty default would mask "no days selected".
        [Display(Name = "Applicable Days")]
        public List<DayOfWeek> SelectedDays { get; set; } = new();

        [Required]
        [Display(Name = "Adjustment Type")]
        public DiscountType AdjustmentType { get; set; } = DiscountType.Percentage;

        [Required]
        [Range(0.01, 100000, ErrorMessage = "Surcharge value must be greater than zero.")]
        [Display(Name = "Surcharge Value")]
        public decimal Value { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Max Surcharge Amount")]
        public decimal? MaxSurchargeAmount { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public DayOfWeekFlags GetDayFlags()
            => SelectedDays.Aggregate(DayOfWeekFlags.None, (acc, d) => acc | d.ToFlag());

        public static PricingRuleViewModel FromEntity(PricingRule rule) => new()
        {
            Id = rule.Id,
            Name = rule.Name,
            Description = rule.Description,
            StartTime = rule.StartTime,
            EndTime = rule.EndTime,
            SelectedDays = Enum.GetValues<DayOfWeek>().Where(d => rule.ApplicableDays.Includes(d)).ToList(),
            AdjustmentType = rule.AdjustmentType,
            Value = rule.Value,
            MaxSurchargeAmount = rule.MaxSurchargeAmount,
            IsActive = rule.IsActive
        };

        public void ApplyTo(PricingRule rule)
        {
            rule.Name = Name.Trim();
            rule.Description = Description;
            rule.StartTime = StartTime;
            rule.EndTime = EndTime;
            rule.ApplicableDays = GetDayFlags();
            rule.AdjustmentType = AdjustmentType;
            rule.Value = Value;
            rule.MaxSurchargeAmount = MaxSurchargeAmount;
            rule.IsActive = IsActive;
        }
    }
}
