using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Cab_Management_System.Models.ViewModels;
using Cab_Management_System.Repositories;

namespace Cab_Management_System.Services
{
    public class DriverRatingService : IDriverRatingService
    {
        private readonly IDriverRatingRepository _ratingRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly ITripRepository _tripRepository;
        private readonly ILogger<DriverRatingService> _logger;

        public DriverRatingService(
            IDriverRatingRepository ratingRepository,
            IDriverRepository driverRepository,
            ITripRepository tripRepository,
            ILogger<DriverRatingService> logger)
        {
            _ratingRepository = ratingRepository;
            _driverRepository = driverRepository;
            _tripRepository = tripRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<DriverRating>> GetRatingsByDriverIdAsync(int driverId)
            => await _ratingRepository.GetRatingsByDriverIdAsync(driverId);

        public async Task<DriverRating?> GetRatingByTripIdAsync(int tripId)
            => await _ratingRepository.GetRatingByTripIdAsync(tripId);

        public async Task<double> GetAverageRatingForDriverAsync(int driverId)
            => await _ratingRepository.GetAverageRatingForDriverAsync(driverId);

        public async Task CreateRatingAsync(DriverRating rating)
        {
            await _ratingRepository.AddAsync(rating);
            await _ratingRepository.SaveChangesAsync();
            _logger.LogInformation("Rating created for Trip {TripId}, Driver {DriverId}, Rating: {Rating}",
                rating.TripId, rating.DriverId, rating.Rating);
        }

        public async Task<bool> CanRateTripAsync(int tripId)
        {
            var trip = await _tripRepository.GetByIdAsync(tripId);
            if (trip == null || trip.Status != TripStatus.Completed)
                return false;

            var existingRating = await _ratingRepository.GetRatingByTripIdAsync(tripId);
            return existingRating == null;
        }

        public async Task<DriverPerformanceViewModel> GetDriverPerformanceAsync(int driverId)
        {
            var driver = await _driverRepository.GetDriverWithEmployeeAsync(driverId);
            if (driver == null)
                throw new KeyNotFoundException($"Driver with ID {driverId} not found.");

            var allTrips = await _tripRepository.FindAsync(t => t.DriverId == driverId);
            var tripList = allTrips.ToList();
            var completedTrips = tripList.Where(t => t.Status == TripStatus.Completed).ToList();
            var ratings = (await _ratingRepository.GetRatingsByDriverIdAsync(driverId)).ToList();

            var model = new DriverPerformanceViewModel
            {
                Driver = driver,
                AverageRating = ratings.Count > 0 ? ratings.Average(r => r.Rating) : 0,
                TotalTrips = tripList.Count,
                CompletedTrips = completedTrips.Count,
                CompletionRate = tripList.Count > 0 ? (double)completedTrips.Count / tripList.Count * 100 : 0,
                TotalRevenue = completedTrips.Sum(t => t.Cost),
                TotalRatings = ratings.Count,
                RecentRatings = ratings.Take(10)
            };

            // Rating distribution (1-5 stars)
            for (int i = 1; i <= 5; i++)
            {
                model.RatingDistribution[i - 1] = ratings.Count(r => r.Rating == i);
            }

            // Monthly trip counts (last 6 months)
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);
            var monthlyTrips = completedTrips
                .Where(t => t.TripDate >= sixMonthsAgo)
                .GroupBy(t => new { t.TripDate.Year, t.TripDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .ToList();

            foreach (var group in monthlyTrips)
            {
                model.MonthLabels.Add($"{new DateTime(group.Key.Year, group.Key.Month, 1):MMM yyyy}");
                model.MonthlyCounts.Add(group.Count());
            }

            return model;
        }
    }
}
