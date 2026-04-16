using CabManagementSystem.Models;
using CabManagementSystem.Models.Enums;
using CabManagementSystem.Models.ViewModels;
using CabManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CabManagementSystem.Areas.Travel.Controllers
{
    [Area("Travel")]
    [Authorize(Roles = nameof(UserRole.TravelManager))]
    public class FeedbackController : Controller
    {
        private readonly ITripFeedbackService _feedbackService;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly ICustomerService _customerService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<FeedbackController> _logger;

        public FeedbackController(
            ITripFeedbackService feedbackService,
            INotificationService notificationService,
            IEmailService emailService,
            ICustomerService customerService,
            UserManager<ApplicationUser> userManager,
            ILogger<FeedbackController> logger)
        {
            _feedbackService = feedbackService;
            _notificationService = notificationService;
            _emailService = emailService;
            _customerService = customerService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index(FeedbackCategory? category, FeedbackStatus? status, string? searchTerm, int page = 1)
        {
            var feedbacks = await _feedbackService.GetFilteredFeedbackAsync(category, status, searchTerm);
            var pageSize = 10;
            var paginatedList = PaginatedList<TripFeedback>.Create(feedbacks, page, pageSize);

            ViewBag.PageIndex = paginatedList.PageIndex;
            ViewBag.TotalPages = paginatedList.TotalPages;
            ViewBag.TotalCount = paginatedList.TotalCount;
            ViewBag.BaseUrl = Url.Action("Index");
            ViewBag.Categories = Enum.GetValues<FeedbackCategory>();
            ViewBag.Statuses = Enum.GetValues<FeedbackStatus>();
            ViewBag.SelectedCategory = category;
            ViewBag.SelectedStatus = status;
            ViewBag.SearchTerm = searchTerm;

            var queryParts = new List<string>();
            if (category.HasValue) queryParts.Add($"category={category}");
            if (status.HasValue) queryParts.Add($"status={status}");
            if (!string.IsNullOrEmpty(searchTerm)) queryParts.Add($"searchTerm={searchTerm}");
            ViewBag.QueryString = queryParts.Count > 0 ? "&" + string.Join("&", queryParts) : "";

            return View(paginatedList);
        }

        public async Task<IActionResult> Details(int id)
        {
            var feedback = await _feedbackService.GetFeedbackByIdAsync(id);
            if (feedback == null) return NotFound();

            return View(feedback);
        }

        [HttpGet]
        public async Task<IActionResult> Respond(int id)
        {
            var feedback = await _feedbackService.GetFeedbackByIdAsync(id);
            if (feedback == null) return NotFound();

            var model = new FeedbackResponseViewModel
            {
                FeedbackId = feedback.Id,
                CustomerName = feedback.Customer.Name,
                Comment = feedback.Comment,
                Rating = feedback.Rating,
                Category = feedback.Category,
                CurrentStatus = feedback.Status,
                RouteInfo = $"{feedback.Trip.Route.Origin} → {feedback.Trip.Route.Destination}",
                TripDate = feedback.Trip.TripDate,
                NewStatus = FeedbackStatus.Resolved
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Respond(FeedbackResponseViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var respondedBy = User.Identity?.Name ?? "Travel Manager";
                await _feedbackService.RespondToFeedbackAsync(model.FeedbackId, model.AdminResponse, respondedBy, model.NewStatus);

                // Notify customer
                try
                {
                    var feedback = await _feedbackService.GetFeedbackByIdAsync(model.FeedbackId);
                    if (feedback != null)
                    {
                        var customer = feedback.Customer;
                        var customerUser = await _userManager.FindByEmailAsync(customer.Email);
                        if (customerUser != null)
                        {
                            await _notificationService.CreateNotificationAsync(
                                customerUser.Id,
                                "Feedback Response",
                                "Your feedback has received a response from our team.",
                                $"/CustomerPortal/Feedback/Details/{model.FeedbackId}");
                        }

                        try
                        {
                            var htmlBody = $@"
                                <h2>Your Feedback Has Been Responded To</h2>
                                <p>Dear {customer.Name},</p>
                                <p>Our team has responded to your feedback for Trip #{feedback.TripId}.</p>
                                <p><strong>Our Response:</strong></p>
                                <p>{model.AdminResponse}</p>
                                <p><strong>Status:</strong> {model.NewStatus}</p>
                                <p>Thank you for your feedback!</p>
                                <p>Cab Management System</p>";

                            await _emailService.SendEmailAsync(customer.Email, "Your Feedback Has Been Responded To", htmlBody);
                        }
                        catch (Exception emailEx)
                        {
                            _logger.LogWarning(emailEx, "Failed to send feedback response email to {Email}", customer.Email);
                        }
                    }
                }
                catch (Exception notifEx)
                {
                    _logger.LogWarning(notifEx, "Failed to send feedback response notification for Feedback {FeedbackId}", model.FeedbackId);
                }

                TempData["SuccessMessage"] = "Response submitted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error responding to feedback {FeedbackId}", model.FeedbackId);
                TempData["ErrorMessage"] = "An error occurred while submitting the response.";
                return View(model);
            }
        }
    }
}
