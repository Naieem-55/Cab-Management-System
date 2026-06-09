using CabManagementSystem.Models;
using CabManagementSystem.Models.Enums;
using CabManagementSystem.Repositories;

namespace CabManagementSystem.Services
{
    public class LoyaltyPointsService : ILoyaltyPointsService
    {
        public const int POINTS_PER_DOLLAR = 1;
        public const int RATING_BONUS = 20;
        public const int FEEDBACK_BONUS = 20;
        public const int SIGNUP_BONUS = 50;
        public const int POINTS_TO_DOLLAR_RATIO = 100; // 100 points = $1

        private readonly ILoyaltyTransactionRepository _transactionRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IDriverRatingRepository _ratingRepository;
        private readonly ITripFeedbackRepository _feedbackRepository;
        private readonly ILogger<LoyaltyPointsService> _logger;

        public LoyaltyPointsService(
            ILoyaltyTransactionRepository transactionRepository,
            ICustomerRepository customerRepository,
            ITripRepository tripRepository,
            IDriverRatingRepository ratingRepository,
            ITripFeedbackRepository feedbackRepository,
            ILogger<LoyaltyPointsService> logger)
        {
            _transactionRepository = transactionRepository;
            _customerRepository = customerRepository;
            _tripRepository = tripRepository;
            _ratingRepository = ratingRepository;
            _feedbackRepository = feedbackRepository;
            _logger = logger;
        }

        public async Task AwardTripPointsAsync(int tripId)
        {
            var trip = await _tripRepository.GetByIdAsync(tripId);
            if (trip == null || !trip.CustomerId.HasValue)
                return;

            var alreadyAwarded = await _transactionRepository.HasTransactionForTripAsync(tripId, LoyaltyTransactionType.Earned);
            if (alreadyAwarded)
                return;

            var customer = await _customerRepository.GetByIdAsync(trip.CustomerId.Value);
            if (customer == null)
                return;

            int points = (int)(trip.Cost * POINTS_PER_DOLLAR);
            if (points <= 0)
                return;

            customer.LoyaltyPoints += points;
            _customerRepository.Update(customer);

            await _transactionRepository.AddAsync(new LoyaltyTransaction
            {
                CustomerId = customer.Id,
                TripId = tripId,
                Points = points,
                Type = LoyaltyTransactionType.Earned,
                Description = $"Earned from Trip #{tripId} completion"
            });

            await _transactionRepository.SaveChangesAsync();
            _logger.LogInformation("Awarded {Points} loyalty points to Customer {CustomerId} for Trip {TripId}",
                points, customer.Id, tripId);
        }

        public async Task AwardRatingBonusAsync(int tripId)
        {
            var trip = await _tripRepository.GetByIdAsync(tripId);
            if (trip == null || !trip.CustomerId.HasValue)
                return;

            var rating = await _ratingRepository.GetRatingByTripIdAsync(tripId);
            if (rating == null || rating.Rating != 5)
                return;

            // Guard duplicate: check transactions for this trip with rating bonus description
            var existing = (await _transactionRepository.GetByCustomerIdAsync(trip.CustomerId.Value))
                .Any(t => t.TripId == tripId && t.Type == LoyaltyTransactionType.Bonus
                          && t.Description.Contains("rating", StringComparison.OrdinalIgnoreCase));
            if (existing)
                return;

            var customer = await _customerRepository.GetByIdAsync(trip.CustomerId.Value);
            if (customer == null)
                return;

            customer.LoyaltyPoints += RATING_BONUS;
            _customerRepository.Update(customer);

            await _transactionRepository.AddAsync(new LoyaltyTransaction
            {
                CustomerId = customer.Id,
                TripId = tripId,
                Points = RATING_BONUS,
                Type = LoyaltyTransactionType.Bonus,
                Description = $"5-star rating bonus for Trip #{tripId}"
            });

            await _transactionRepository.SaveChangesAsync();
            _logger.LogInformation("Awarded {Points} rating bonus to Customer {CustomerId} for Trip {TripId}",
                RATING_BONUS, customer.Id, tripId);
        }

        public async Task AwardFeedbackBonusAsync(int tripId)
        {
            var trip = await _tripRepository.GetByIdAsync(tripId);
            if (trip == null || !trip.CustomerId.HasValue)
                return;

            var feedback = await _feedbackRepository.GetFeedbackByTripIdAsync(tripId);
            if (feedback == null || feedback.Rating != 5)
                return;

            // Guard duplicate
            var existing = (await _transactionRepository.GetByCustomerIdAsync(trip.CustomerId.Value))
                .Any(t => t.TripId == tripId && t.Type == LoyaltyTransactionType.Bonus
                          && t.Description.Contains("feedback", StringComparison.OrdinalIgnoreCase));
            if (existing)
                return;

            var customer = await _customerRepository.GetByIdAsync(trip.CustomerId.Value);
            if (customer == null)
                return;

            customer.LoyaltyPoints += FEEDBACK_BONUS;
            _customerRepository.Update(customer);

            await _transactionRepository.AddAsync(new LoyaltyTransaction
            {
                CustomerId = customer.Id,
                TripId = tripId,
                Points = FEEDBACK_BONUS,
                Type = LoyaltyTransactionType.Bonus,
                Description = $"5-star feedback bonus for Trip #{tripId}"
            });

            await _transactionRepository.SaveChangesAsync();
            _logger.LogInformation("Awarded {Points} feedback bonus to Customer {CustomerId} for Trip {TripId}",
                FEEDBACK_BONUS, customer.Id, tripId);
        }

        public async Task AwardSignupBonusAsync(int customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
                return;

            // Guard duplicate signup bonus
            var existing = (await _transactionRepository.GetByCustomerIdAsync(customerId))
                .Any(t => t.Type == LoyaltyTransactionType.SignupBonus);
            if (existing)
                return;

            customer.LoyaltyPoints += SIGNUP_BONUS;
            _customerRepository.Update(customer);

            await _transactionRepository.AddAsync(new LoyaltyTransaction
            {
                CustomerId = customerId,
                TripId = null,
                Points = SIGNUP_BONUS,
                Type = LoyaltyTransactionType.SignupBonus,
                Description = "Welcome bonus for registration"
            });

            await _transactionRepository.SaveChangesAsync();
            _logger.LogInformation("Awarded {Points} signup bonus to Customer {CustomerId}", SIGNUP_BONUS, customerId);
        }

        public async Task<bool> RedeemPointsAsync(int customerId, int points, int tripId, decimal discountAmount)
        {
            if (points <= 0)
                return false;

            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null || customer.LoyaltyPoints < points)
                return false;

            customer.LoyaltyPoints -= points;
            _customerRepository.Update(customer);

            await _transactionRepository.AddAsync(new LoyaltyTransaction
            {
                CustomerId = customerId,
                TripId = tripId,
                Points = -points,
                Type = LoyaltyTransactionType.Redeemed,
                Description = $"Redeemed {points} pts for {discountAmount:C} discount on Trip #{tripId}"
            });

            await _transactionRepository.SaveChangesAsync();
            _logger.LogInformation("Customer {CustomerId} redeemed {Points} points ({Discount:C}) on Trip {TripId}",
                customerId, points, discountAmount, tripId);
            return true;
        }

        public async Task<int> GetBalanceAsync(int customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            return customer?.LoyaltyPoints ?? 0;
        }

        public async Task<IEnumerable<LoyaltyTransaction>> GetHistoryAsync(int customerId)
        {
            return await _transactionRepository.GetByCustomerIdAsync(customerId);
        }
    }
}
