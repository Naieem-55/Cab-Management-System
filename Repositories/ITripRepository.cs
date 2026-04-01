using CabManagementSystem.Models;
using CabManagementSystem.Models.Enums;

namespace CabManagementSystem.Repositories
{
    public interface ITripRepository : IRepository<Trip>
    {
        Task<Trip?> GetTripWithDetailsAsync(int id);
        Task<IEnumerable<Trip>> GetTripsByStatusAsync(TripStatus status);
        Task<IEnumerable<Trip>> GetTripsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Trip>> GetTripsByCustomerIdAsync(int customerId);
    }
}
