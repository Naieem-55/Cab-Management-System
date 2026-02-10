using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;

namespace Cab_Management_System.Services
{
    public interface ITripService
    {
        Task<IEnumerable<Trip>> GetAllTripsAsync();
        Task<Trip?> GetTripByIdAsync(int id);
        Task<Trip?> GetTripWithDetailsAsync(int id);
        Task<IEnumerable<Trip>> GetTripsByStatusAsync(TripStatus status);
        Task<IEnumerable<Trip>> GetTripsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Trip>> SearchTripsAsync(string searchTerm);
        Task CreateTripAsync(Trip trip);
        Task UpdateTripAsync(Trip trip);
        Task UpdateTripStatusAsync(int tripId, TripStatus newStatus);
        Task DeleteTripAsync(int id);
        Task<int> GetTripCountAsync();
        Task<int> GetActiveTripCountAsync();
        Task<IEnumerable<Trip>> GetTripsByCustomerIdAsync(int customerId);
        Task<int> GetTripCountByCustomerIdAsync(int customerId);
        Task<int> GetActiveTripCountByCustomerIdAsync(int customerId);
        Task<decimal> GetTotalSpentByCustomerIdAsync(int customerId);
    }
}
