using Cab_Management_System.Models;

namespace Cab_Management_System.Services
{
    public interface IRouteService
    {
        Task<IEnumerable<Models.Route>> GetAllRoutesAsync();
        Task<Models.Route?> GetRouteByIdAsync(int id);
        Task<Models.Route?> GetRouteWithTripsAsync(int id);
        Task<IEnumerable<Models.Route>> SearchRoutesAsync(string searchTerm);
        Task CreateRouteAsync(Models.Route route);
        Task UpdateRouteAsync(Models.Route route);
        Task DeleteRouteAsync(int id);
        Task<int> GetRouteCountAsync();
        Task<bool> CanDeleteAsync(int id);
        Task<int> GetTripCountAsync(int id);
    }
}
