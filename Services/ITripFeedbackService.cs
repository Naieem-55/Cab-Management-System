using CabManagementSystem.Models;
using CabManagementSystem.Models.Enums;

namespace CabManagementSystem.Services
{
    public interface ITripFeedbackService
    {
        Task<IEnumerable<TripFeedback>> GetAllFeedbackAsync();
        Task<TripFeedback?> GetFeedbackByIdAsync(int id);
        Task<TripFeedback?> GetFeedbackByTripIdAsync(int tripId);
        Task<IEnumerable<TripFeedback>> GetFeedbackByCustomerIdAsync(int customerId);
        Task<IEnumerable<TripFeedback>> GetFilteredFeedbackAsync(FeedbackCategory? category, FeedbackStatus? status, string? searchTerm);
        Task CreateFeedbackAsync(TripFeedback feedback);
        Task<bool> CanSubmitFeedbackAsync(int tripId, int customerId);
        Task RespondToFeedbackAsync(int feedbackId, string response, string respondedBy, FeedbackStatus newStatus);
        Task<double> GetAverageRatingAsync();
        Task<int> GetOpenFeedbackCountAsync();
        Task<int> GetTotalFeedbackCountAsync();
    }
}
