using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;

namespace Cab_Management_System.Repositories
{
    public interface ITripRepository : IRepository<Trip>
    {
        Task<Trip?> GetTripWithDetailsAsync(int id);
        Task<IEnumerable<Trip>> GetTripsByStatusAsync(TripStatus status);
        Task<IEnumerable<Trip>> GetTripsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Trip>> GetTripsByCustomerIdAsync(int customerId);
    }
}
