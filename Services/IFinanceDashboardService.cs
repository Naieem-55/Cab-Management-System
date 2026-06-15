using CabManagementSystem.Models.ViewModels;

namespace CabManagementSystem.Services
{
    public interface IFinanceDashboardService
    {
        Task<FinanceDashboardViewModel> GetFinanceDashboardAsync(int months = 6);
    }
}
