using CabManagementSystem.Models;

namespace CabManagementSystem.Repositories
{
    public interface IRouteRepository : IRepository<Models.Route>
    {
        Task<IEnumerable<Models.Route>> SearchRoutesAsync(string searchTerm);
        Task<Models.Route?> GetRouteWithTripsAsync(int id);
    }
}
