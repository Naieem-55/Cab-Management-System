using Cab_Management_System.Models;
using Cab_Management_System.Repositories;
using Microsoft.Extensions.Logging;

namespace Cab_Management_System.Services
{
    public class RouteService : IRouteService
    {
        private readonly IRouteRepository _routeRepository;
        private readonly ITripRepository _tripRepository;
        private readonly ILogger<RouteService> _logger;

        public RouteService(IRouteRepository routeRepository, ITripRepository tripRepository, ILogger<RouteService> logger)
        {
            _routeRepository = routeRepository;
            _tripRepository = tripRepository;
            _logger = logger;
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
            _logger.LogInformation("Created Route with ID {Id}", route.Id);
        }

        public async Task UpdateRouteAsync(Models.Route route)
        {
            _routeRepository.Update(route);
            await _routeRepository.SaveChangesAsync();
            _logger.LogInformation("Updated Route with ID {Id}", route.Id);
        }

        public async Task DeleteRouteAsync(int id)
        {
            var route = await _routeRepository.GetByIdAsync(id);
            if (route == null)
            {
                _logger.LogWarning("Route with ID {Id} not found", id);
                throw new KeyNotFoundException($"Route with ID {id} not found.");
            }

            _routeRepository.Remove(route);
            await _routeRepository.SaveChangesAsync();
            _logger.LogInformation("Deleted Route with ID {Id}", id);
        }

        public async Task<int> GetRouteCountAsync()
        {
            return await _routeRepository.CountAsync();
        }

        public async Task<bool> CanDeleteAsync(int id)
        {
            var tripCount = await _tripRepository.CountAsync(t => t.RouteId == id);
            return tripCount == 0;
        }

        public async Task<int> GetTripCountAsync(int id)
        {
            return await _tripRepository.CountAsync(t => t.RouteId == id);
        }
    }
}
