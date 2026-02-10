using Cab_Management_System.Models;
using Cab_Management_System.Models.ViewModels;

namespace Cab_Management_System.Services
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
