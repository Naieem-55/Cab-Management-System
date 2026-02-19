using Cab_Management_System.Data;
using Cab_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cab_Management_System.Repositories
{
    public class DriverRatingRepository : Repository<DriverRating>, IDriverRatingRepository
    {
        public DriverRatingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<DriverRating>> GetAllAsync()
        {
            return await _context.DriverRatings
                .Include(dr => dr.Trip)
                .Include(dr => dr.Driver).ThenInclude(d => d.Employee)
                .OrderByDescending(dr => dr.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<DriverRating>> GetRatingsByDriverIdAsync(int driverId)
        {
            return await _context.DriverRatings
                .Include(dr => dr.Trip).ThenInclude(t => t.Route)
                .Where(dr => dr.DriverId == driverId)
                .OrderByDescending(dr => dr.CreatedDate)
                .ToListAsync();
        }

        public async Task<DriverRating?> GetRatingByTripIdAsync(int tripId)
        {
            return await _context.DriverRatings
                .Include(dr => dr.Driver).ThenInclude(d => d.Employee)
                .FirstOrDefaultAsync(dr => dr.TripId == tripId);
        }

        public async Task<double> GetAverageRatingForDriverAsync(int driverId)
        {
            var ratings = await _context.DriverRatings
                .Where(dr => dr.DriverId == driverId)
                .Select(dr => dr.Rating)
                .ToListAsync();

            return ratings.Count > 0 ? ratings.Average() : 0;
        }

        public async Task<List<(int DriverId, double AvgRating)>> GetTopRatedDriverIdsAsync(int count)
        {
            var results = await _context.DriverRatings
                .GroupBy(dr => dr.DriverId)
                .Select(g => new { DriverId = g.Key, AvgRating = g.Average(r => r.Rating) })
                .OrderByDescending(x => x.AvgRating)
                .Take(count)
                .ToListAsync();

            return results.Select(x => (x.DriverId, x.AvgRating)).ToList();
        }
    }
}
