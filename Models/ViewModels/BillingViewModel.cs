using System.ComponentModel.DataAnnotations;
using Cab_Management_System.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cab_Management_System.Models.ViewModels
{
    public class BillingViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Trip")]
        public int TripId { get; set; }

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

        public SelectList? AvailableTrips { get; set; }

        [Display(Name = "Customer Name")]
        public string? CustomerName { get; set; }
    }
}
