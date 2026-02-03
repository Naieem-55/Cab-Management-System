using Cab_Management_System.Models.ViewModels;

namespace Cab_Management_System.Services
{
    public interface IDashboardService
    {
        Task<AdminDashboardViewModel> GetAdminDashboardAsync();
        Task<FinanceDashboardViewModel> GetFinanceDashboardAsync();
        Task<HRDashboardViewModel> GetHRDashboardAsync();
        Task<TravelDashboardViewModel> GetTravelDashboardAsync();
    }
}
