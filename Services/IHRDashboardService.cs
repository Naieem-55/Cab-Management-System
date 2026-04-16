using CabManagementSystem.Models.ViewModels;

namespace CabManagementSystem.Services
{
    public interface IHRDashboardService
    {
        Task<HRDashboardViewModel> GetHRDashboardAsync();
    }
}
