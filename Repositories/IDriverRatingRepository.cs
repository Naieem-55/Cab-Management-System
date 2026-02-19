using Cab_Management_System.Models;

namespace Cab_Management_System.Repositories
{
    public interface IDriverRatingRepository : IRepository<DriverRating>
    {
        Task<IEnumerable<DriverRating>> GetRatingsByDriverIdAsync(int driverId);
        Task<DriverRating?> GetRatingByTripIdAsync(int tripId);
        Task<double> GetAverageRatingForDriverAsync(int driverId);
        Task<List<(int DriverId, double AvgRating)>> GetTopRatedDriverIdsAsync(int count);
    }
}
