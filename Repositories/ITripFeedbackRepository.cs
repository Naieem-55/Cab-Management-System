using CabManagementSystem.Models;
using CabManagementSystem.Models.Enums;

namespace CabManagementSystem.Repositories
{
    public interface ITripFeedbackRepository : IRepository<TripFeedback>
    {
        Task<TripFeedback?> GetFeedbackByTripIdAsync(int tripId);
        Task<IEnumerable<TripFeedback>> GetFeedbackByCustomerIdAsync(int customerId);
        Task<double> GetAverageRatingAsync();
        Task<int> GetOpenFeedbackCountAsync();
    }
}
