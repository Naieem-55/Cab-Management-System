using System.ComponentModel.DataAnnotations;
using Cab_Management_System.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cab_Management_System.Models.ViewModels
{
    public class DriverViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "License Number")]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "License Expiry")]
        public DateTime LicenseExpiry { get; set; }

        [Required]
        public DriverStatus Status { get; set; } = DriverStatus.Available;

        public SelectList? AvailableEmployees { get; set; }

        [Display(Name = "Employee Name")]
        public string? EmployeeName { get; set; }
    }
}
