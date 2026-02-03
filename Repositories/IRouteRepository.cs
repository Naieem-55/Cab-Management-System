using Cab_Management_System.Models;

namespace Cab_Management_System.Repositories
{
    public interface IRouteRepository : IRepository<Models.Route>
    {
        Task<IEnumerable<Models.Route>> SearchRoutesAsync(string searchTerm);
        Task<Models.Route?> GetRouteWithTripsAsync(int id);
    }
}
