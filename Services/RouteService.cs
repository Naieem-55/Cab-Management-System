using Cab_Management_System.Models;
using Cab_Management_System.Repositories;

namespace Cab_Management_System.Services
{
    public class RouteService : IRouteService
    {
        private readonly IRouteRepository _routeRepository;

        public RouteService(IRouteRepository routeRepository)
        {
            _routeRepository = routeRepository;
        }

        public async Task<IEnumerable<Models.Route>> GetAllRoutesAsync()
        {
            return await _routeRepository.GetAllAsync();
        }

        public async Task<Models.Route?> GetRouteByIdAsync(int id)
        {
            return await _routeRepository.GetByIdAsync(id);
        }

        public async Task<Models.Route?> GetRouteWithTripsAsync(int id)
        {
            return await _routeRepository.GetRouteWithTripsAsync(id);
        }

        public async Task<IEnumerable<Models.Route>> SearchRoutesAsync(string searchTerm)
        {
            return await _routeRepository.SearchRoutesAsync(searchTerm);
        }

        public async Task CreateRouteAsync(Models.Route route)
        {
            await _routeRepository.AddAsync(route);
            await _routeRepository.SaveChangesAsync();
        }

        public async Task UpdateRouteAsync(Models.Route route)
        {
            _routeRepository.Update(route);
            await _routeRepository.SaveChangesAsync();
        }

        public async Task DeleteRouteAsync(int id)
        {
            var route = await _routeRepository.GetByIdAsync(id);
            if (route == null)
                throw new KeyNotFoundException($"Route with ID {id} not found.");

            _routeRepository.Remove(route);
            await _routeRepository.SaveChangesAsync();
        }

        public async Task<int> GetRouteCountAsync()
        {
            return await _routeRepository.CountAsync();
        }
    }
}
