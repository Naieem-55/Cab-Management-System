using CabManagementSystem.Models;
using CabManagementSystem.Models.Enums;
using CabManagementSystem.Models.ViewModels;
using CabManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CabManagementSystem.Areas.CustomerPortal.Controllers
{
    [Area("CustomerPortal")]
    [Authorize(Roles = nameof(UserRole.Customer))]
    public class FeedbackController : Controller
    {
        private readonly ITripFeedbackService _feedbackService;
        private readonly ITripService _tripService;
        private readonly ICustomerService _customerService;
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<FeedbackController> _logger;

        public FeedbackController(
            ITripFeedbackService feedbackService,
            ITripService tripService,
            ICustomerService customerService,
            INotificationService notificationService,
            UserManager<ApplicationUser> userManager,
            ILogger<FeedbackController> logger)
        {
            _feedbackService = feedbackService;
            _tripService = tripService;
            _customerService = customerService;
            _notificationService = notificationService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var customer = await GetCurrentCustomerAsync();
            if (customer == null) return RedirectToAction("Login", "Account", new { area = "" });

            var feedbacks = await _feedbackService.GetFeedbackByCustomerIdAsync(customer.Id);
            var pageSize = 10;
            var paginatedList = PaginatedList<TripFeedback>.Create(feedbacks, page, pageSize);

            ViewBag.PageIndex = paginatedList.PageIndex;
            ViewBag.TotalPages = paginatedList.TotalPages;
            ViewBag.TotalCount = paginatedList.TotalCount;
            ViewBag.BaseUrl = Url.Action("Index");
            ViewBag.QueryString = "";

            return View(paginatedList);
        }

        public async Task<IActionResult> Details(int id)
        {
            var customer = await GetCurrentCustomerAsync();
            if (customer == null) return RedirectToAction("Login", "Account", new { area = "" });

            var feedback = await _feedbackService.GetFeedbackByIdAsync(id);
            if (feedback == null || feedback.CustomerId != customer.Id)
                return NotFound();

            return View(feedback);
        }

        [HttpGet]
        public async Task<IActionResult> Submit(int tripId)
        {
            var customer = await GetCurrentCustomerAsync();
            if (customer == null) return RedirectToAction("Login", "Account", new { area = "" });

            var canSubmit = await _feedbackService.CanSubmitFeedbackAsync(tripId, customer.Id);
            if (!canSubmit)
            {
                TempData["ErrorMessage"] = "You cannot submit feedback for this trip.";
                return RedirectToAction("Index", "Trip");
            }

            var trip = await _tripService.GetTripWithDetailsAsync(tripId);
            if (trip == null) return NotFound();

            var model = new SubmitFeedbackViewModel
            {
                TripId = tripId,
                DriverName = trip.Driver?.Employee?.Name ?? "N/A",
                RouteInfo = $"{trip.Route.Origin} → {trip.Route.Destination}",
                TripDate = trip.TripDate
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(SubmitFeedbackViewModel model)
        {
            var customer = await GetCurrentCustomerAsync();
            if (customer == null) return RedirectToAction("Login", "Account", new { area = "" });

            if (!ModelState.IsValid)
            {
                var trip = await _tripService.GetTripWithDetailsAsync(model.TripId);
                if (trip != null)
                {
                    model.DriverName = trip.Driver?.Employee?.Name ?? "N/A";
                    model.RouteInfo = $"{trip.Route.Origin} → {trip.Route.Destination}";
                    model.TripDate = trip.TripDate;
                }
                return View(model);
            }

            var canSubmit = await _feedbackService.CanSubmitFeedbackAsync(model.TripId, customer.Id);
            if (!canSubmit)
            {
                TempData["ErrorMessage"] = "You cannot submit feedback for this trip.";
                return RedirectToAction("Index", "Trip");
            }

            try
            {
                var feedback = new TripFeedback
                {
                    TripId = model.TripId,
                    CustomerId = customer.Id,
                    Rating = model.Rating,
                    Category = model.Category,
                    Comment = model.Comment,
                    Status = FeedbackStatus.Open
                };

                await _feedbackService.CreateFeedbackAsync(feedback);

                // Notify admins
                try
                {
                    var admins = await _userManager.GetUsersInRoleAsync(nameof(UserRole.Admin));
                    foreach (var admin in admins)
                    {
                        await _notificationService.CreateNotificationAsync(
                            admin.Id,
                            "New Customer Feedback",
                            $"Customer {customer.Name} submitted feedback for Trip #{model.TripId}.",
                            $"/Admin/Feedback/Details/{feedback.Id}");
                    }

                    var travelManagers = await _userManager.GetUsersInRoleAsync(nameof(UserRole.TravelManager));
                    foreach (var manager in travelManagers)
                    {
                        await _notificationService.CreateNotificationAsync(
                            manager.Id,
                            "New Customer Feedback",
                            $"Customer {customer.Name} submitted feedback for Trip #{model.TripId}.",
                            $"/Travel/Feedback/Details/{feedback.Id}");
                    }
                }
                catch (Exception notifEx)
                {
                    _logger.LogWarning(notifEx, "Failed to send feedback notifications for Trip {TripId}", model.TripId);
                }

                TempData["SuccessMessage"] = "Thank you! Your feedback has been submitted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting feedback for Trip {TripId}", model.TripId);
                TempData["ErrorMessage"] = "An error occurred while submitting your feedback.";
                return View(model);
            }
        }

        private async Task<Customer?> GetCurrentCustomerAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.Email == null) return null;
            return await _customerService.GetCustomerByEmailAsync(user.Email);
        }
    }
}
