using CabManagementSystem.Models;
using CabManagementSystem.Models.ViewModels;

namespace CabManagementSystem.Services
{
    public interface IDriverRatingService
    {
        Task<IEnumerable<DriverRating>> GetRatingsByDriverIdAsync(int driverId);
        Task<DriverRating?> GetRatingByTripIdAsync(int tripId);
        Task<double> GetAverageRatingForDriverAsync(int driverId);
        Task CreateRatingAsync(DriverRating rating);
        Task<bool> CanRateTripAsync(int tripId);
        Task<DriverPerformanceViewModel> GetDriverPerformanceAsync(int driverId);
    }
}
