using CabManagementSystem.Models;

namespace CabManagementSystem.Services
{
    public interface ILoyaltyPointsService
    {
        Task AwardTripPointsAsync(int tripId);
        Task AwardRatingBonusAsync(int tripId);
        Task AwardFeedbackBonusAsync(int tripId);
        Task AwardSignupBonusAsync(int customerId);
        Task<bool> RedeemPointsAsync(int customerId, int points, int tripId, decimal discountAmount);
        Task<int> GetBalanceAsync(int customerId);
        Task<IEnumerable<LoyaltyTransaction>> GetHistoryAsync(int customerId);
    }
}
