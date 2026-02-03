using Cab_Management_System.Data;
using Cab_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cab_Management_System.Repositories
{
    public class RouteRepository : Repository<Models.Route>, IRouteRepository
    {
        public RouteRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Models.Route>> GetAllAsync()
            => await _dbSet.OrderBy(r => r.Origin).ToListAsync();

        public async Task<IEnumerable<Models.Route>> SearchRoutesAsync(string searchTerm)
            => await _dbSet.Where(r => r.Origin.Contains(searchTerm) ||
                                       r.Destination.Contains(searchTerm))
                           .OrderBy(r => r.Origin)
                           .ToListAsync();

        public async Task<Models.Route?> GetRouteWithTripsAsync(int id)
            => await _dbSet.Include(r => r.Trips)
                           .FirstOrDefaultAsync(r => r.Id == id);
    }
}
