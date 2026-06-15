using CabManagementSystem.Models.ViewModels;

namespace CabManagementSystem.Services
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardViewModel> GetAdminDashboardAsync(int months = 6);
    }
}
