using CabManagementSystem.Models.ViewModels;

namespace CabManagementSystem.Services
{
    public interface IDashboardService
    {
        Task<AdminDashboardViewModel> GetAdminDashboardAsync();
        Task<FinanceDashboardViewModel> GetFinanceDashboardAsync();
        Task<HRDashboardViewModel> GetHRDashboardAsync();
        Task<TravelDashboardViewModel> GetTravelDashboardAsync();
    }
}
