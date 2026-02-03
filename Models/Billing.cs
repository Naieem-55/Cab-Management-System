using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cab_Management_System.Models.Enums;

namespace Cab_Management_System.Models
{
    public class Billing
    {
        public int Id { get; set; }

        [Required]
        public int TripId { get; set; }

        [ForeignKey("TripId")]
        public Trip Trip { get; set; } = null!;

        [Required]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; }

        [Required]
        [Display(Name = "Payment Method")]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    }
}
