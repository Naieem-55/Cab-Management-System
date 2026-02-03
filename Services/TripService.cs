using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Cab_Management_System.Repositories;

namespace Cab_Management_System.Services
{
    public class TripService : ITripService
    {
        private readonly ITripRepository _tripRepository;

        public TripService(ITripRepository tripRepository)
        {
            _tripRepository = tripRepository;
        }

        public async Task<IEnumerable<Trip>> GetAllTripsAsync()
        {
            return await _tripRepository.GetAllAsync();
        }

        public async Task<Trip?> GetTripByIdAsync(int id)
        {
            return await _tripRepository.GetByIdAsync(id);
        }

        public async Task<Trip?> GetTripWithDetailsAsync(int id)
        {
            return await _tripRepository.GetTripWithDetailsAsync(id);
        }

        public async Task<IEnumerable<Trip>> GetTripsByStatusAsync(TripStatus status)
        {
            return await _tripRepository.GetTripsByStatusAsync(status);
        }

        public async Task<IEnumerable<Trip>> GetTripsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _tripRepository.GetTripsByDateRangeAsync(startDate, endDate);
        }

        public async Task<IEnumerable<Trip>> SearchTripsAsync(string searchTerm)
        {
            return await _tripRepository.FindAsync(t =>
                t.CustomerName.Contains(searchTerm));
        }

        public async Task CreateTripAsync(Trip trip)
        {
            await _tripRepository.AddAsync(trip);
            await _tripRepository.SaveChangesAsync();
        }

        public async Task UpdateTripAsync(Trip trip)
        {
            _tripRepository.Update(trip);
            await _tripRepository.SaveChangesAsync();
        }

        public async Task DeleteTripAsync(int id)
        {
            var trip = await _tripRepository.GetByIdAsync(id);
            if (trip == null)
                throw new KeyNotFoundException($"Trip with ID {id} not found.");

            _tripRepository.Remove(trip);
            await _tripRepository.SaveChangesAsync();
        }

        public async Task<int> GetTripCountAsync()
        {
            return await _tripRepository.CountAsync();
        }

        public async Task<int> GetActiveTripCountAsync()
        {
            return await _tripRepository.CountAsync(t => t.Status == TripStatus.InProgress);
        }
    }
}
