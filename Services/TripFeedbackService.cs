using CabManagementSystem.Models;
using CabManagementSystem.Models.Enums;
using CabManagementSystem.Repositories;

namespace CabManagementSystem.Services
{
    public class TripFeedbackService : ITripFeedbackService
    {
        private readonly ITripFeedbackRepository _feedbackRepository;
        private readonly ITripRepository _tripRepository;
        private readonly ILoyaltyPointsService _loyaltyService;
        private readonly ILogger<TripFeedbackService> _logger;

        public TripFeedbackService(
            ITripFeedbackRepository feedbackRepository,
            ITripRepository tripRepository,
            ILoyaltyPointsService loyaltyService,
            ILogger<TripFeedbackService> logger)
        {
            _feedbackRepository = feedbackRepository;
            _tripRepository = tripRepository;
            _loyaltyService = loyaltyService;
            _logger = logger;
        }

        public async Task<IEnumerable<TripFeedback>> GetAllFeedbackAsync()
            => await _feedbackRepository.GetAllAsync();

        public async Task<TripFeedback?> GetFeedbackByIdAsync(int id)
        {
            var all = await _feedbackRepository.GetAllAsync();
            return all.FirstOrDefault(f => f.Id == id);
        }

        public async Task<TripFeedback?> GetFeedbackByTripIdAsync(int tripId)
            => await _feedbackRepository.GetFeedbackByTripIdAsync(tripId);

        public async Task<IEnumerable<TripFeedback>> GetFeedbackByCustomerIdAsync(int customerId)
            => await _feedbackRepository.GetFeedbackByCustomerIdAsync(customerId);

        public async Task<IEnumerable<TripFeedback>> GetFilteredFeedbackAsync(
            FeedbackCategory? category, FeedbackStatus? status, string? searchTerm)
        {
            var feedbacks = await _feedbackRepository.GetAllAsync();
            var query = feedbacks.AsQueryable();

            if (category.HasValue)
                query = query.Where(f => f.Category == category.Value);

            if (status.HasValue)
                query = query.Where(f => f.Status == status.Value);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(f =>
                    f.Comment.ToLower().Contains(term) ||
                    f.Customer.Name.ToLower().Contains(term) ||
                    f.Trip.Route.Origin.ToLower().Contains(term) ||
                    f.Trip.Route.Destination.ToLower().Contains(term));
            }

            return query.ToList();
        }

        public async Task CreateFeedbackAsync(TripFeedback feedback)
        {
            await _feedbackRepository.AddAsync(feedback);
            await _feedbackRepository.SaveChangesAsync();
            _logger.LogInformation("Feedback created for Trip {TripId} by Customer {CustomerId}, Rating: {Rating}, Category: {Category}",
                feedback.TripId, feedback.CustomerId, feedback.Rating, feedback.Category);

            // Award loyalty bonus if 5 stars
            try
            {
                await _loyaltyService.AwardFeedbackBonusAsync(feedback.TripId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to award feedback loyalty bonus for Trip {TripId}", feedback.TripId);
            }
        }

        public async Task<bool> CanSubmitFeedbackAsync(int tripId, int customerId)
        {
            var trip = await _tripRepository.GetByIdAsync(tripId);
            if (trip == null || trip.Status != TripStatus.Completed)
                return false;

            if (trip.CustomerId != customerId)
                return false;

            var existing = await _feedbackRepository.GetFeedbackByTripIdAsync(tripId);
            return existing == null;
        }

        public async Task RespondToFeedbackAsync(int feedbackId, string response, string respondedBy, FeedbackStatus newStatus)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(feedbackId);
            if (feedback == null)
                throw new KeyNotFoundException($"Feedback with ID {feedbackId} not found.");

            feedback.AdminResponse = response;
            feedback.RespondedBy = respondedBy;
            feedback.RespondedDate = DateTime.Now;
            feedback.Status = newStatus;

            _feedbackRepository.Update(feedback);
            await _feedbackRepository.SaveChangesAsync();
            _logger.LogInformation("Feedback {FeedbackId} responded by {RespondedBy}, new status: {Status}",
                feedbackId, respondedBy, newStatus);
        }

        public async Task<double> GetAverageRatingAsync()
            => await _feedbackRepository.GetAverageRatingAsync();

        public async Task<int> GetOpenFeedbackCountAsync()
            => await _feedbackRepository.GetOpenFeedbackCountAsync();

        public async Task<int> GetTotalFeedbackCountAsync()
            => await _feedbackRepository.CountAsync();
    }
}
