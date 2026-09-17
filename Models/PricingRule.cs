using System.ComponentModel.DataAnnotations;
using CabManagementSystem.Models.Enums;

namespace CabManagementSystem.Models
{
    /// <summary>
    /// Time-of-day surcharge applied on top of a route's base cost (peak hour, night, etc.).
    /// A window whose EndTime is at or before StartTime wraps past midnight (e.g. 22:00 → 06:00);
    /// for such windows, ApplicableDays refers to the day the window starts on.
    /// </summary>
    public class PricingRule : BaseEntity
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
        public TimeSpan StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "End Time")]
        public TimeSpan EndTime { get; set; }

        [Display(Name = "Applicable Days")]
        public DayOfWeekFlags ApplicableDays { get; set; } = DayOfWeekFlags.All;

        [Required]
        [Display(Name = "Adjustment Type")]
        public DiscountType AdjustmentType { get; set; } = DiscountType.Percentage;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Surcharge value must be greater than zero.")]
        [Display(Name = "Surcharge Value")]
        public decimal Value { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Max Surcharge Amount")]
        public decimal? MaxSurchargeAmount { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public bool WrapsMidnight => EndTime <= StartTime;

        /// <summary>True when the given moment falls inside this rule's window.</summary>
        public bool AppliesAt(DateTime moment)
        {
            if (!IsActive) return false;

            var time = moment.TimeOfDay;
            if (!WrapsMidnight)
                return ApplicableDays.Includes(moment.DayOfWeek) && time >= StartTime && time < EndTime;

            // Evening part belongs to today's window; early-morning part belongs to yesterday's.
            if (time >= StartTime)
                return ApplicableDays.Includes(moment.DayOfWeek);
            if (time < EndTime)
                return ApplicableDays.Includes(moment.AddDays(-1).DayOfWeek);
            return false;
        }

        public decimal CalculateSurcharge(decimal baseFare)
        {
            var amount = AdjustmentType == DiscountType.Percentage
                ? baseFare * (Value / 100m)
                : Value;

            if (MaxSurchargeAmount.HasValue && amount > MaxSurchargeAmount.Value)
                amount = MaxSurchargeAmount.Value;

            return Math.Round(amount, 2, MidpointRounding.AwayFromZero);
        }

        public string AdjustmentDisplay => AdjustmentType == DiscountType.Percentage
            ? $"+{Value:0.##}%" + (MaxSurchargeAmount.HasValue ? $" (max {MaxSurchargeAmount:C})" : "")
            : $"+{Value:C}";

        public string WindowDisplay => $"{StartTime:hh\\:mm} – {EndTime:hh\\:mm}"
            + (WrapsMidnight ? " (next day)" : "");
    }
}
