using CabManagementSystem.Models.ViewModels;

namespace CabManagementSystem.Services
{
    public interface ITravelDashboardService
    {
        Task<TravelDashboardViewModel> GetTravelDashboardAsync();
    }
}
