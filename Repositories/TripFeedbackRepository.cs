using CabManagementSystem.Data;
using CabManagementSystem.Models;
using CabManagementSystem.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CabManagementSystem.Repositories
{
    public class TripFeedbackRepository : Repository<TripFeedback>, ITripFeedbackRepository
    {
        public TripFeedbackRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<TripFeedback>> GetAllAsync()
        {
            return await _context.TripFeedbacks
                .Include(tf => tf.Trip).ThenInclude(t => t.Route)
                .Include(tf => tf.Customer)
                .OrderByDescending(tf => tf.CreatedDate)
                .ToListAsync();
        }

        public async Task<TripFeedback?> GetFeedbackByTripIdAsync(int tripId)
        {
            return await _context.TripFeedbacks
                .Include(tf => tf.Trip).ThenInclude(t => t.Route)
                .Include(tf => tf.Customer)
                .FirstOrDefaultAsync(tf => tf.TripId == tripId);
        }

        public async Task<IEnumerable<TripFeedback>> GetFeedbackByCustomerIdAsync(int customerId)
        {
            return await _context.TripFeedbacks
                .Include(tf => tf.Trip).ThenInclude(t => t.Route)
                .Where(tf => tf.CustomerId == customerId)
                .OrderByDescending(tf => tf.CreatedDate)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingAsync()
        {
            var ratings = await _context.TripFeedbacks
                .Select(tf => tf.Rating)
                .ToListAsync();

            return ratings.Count > 0 ? ratings.Average() : 0;
        }

        public async Task<int> GetOpenFeedbackCountAsync()
        {
            return await _context.TripFeedbacks
                .CountAsync(tf => tf.Status == FeedbackStatus.Open);
        }
    }
}
