using CabManagementSystem.Models;
using CabManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CabManagementSystem.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationController(INotificationService notificationService, UserManager<ApplicationUser> userManager)
        {
            _notificationService = notificationService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var notifications = await _notificationService.GetUserNotificationsAsync(user.Id, 100);
            return View(notifications);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return RedirectToAction(nameof(Index));
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReadAll()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            await _notificationService.MarkAllAsReadAsync(user.Id);
            TempData["SuccessMessage"] = "All notifications marked as read.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Json(new { count = 0 });

            var count = await _notificationService.GetUnreadCountAsync(user.Id);
            return Json(new { count });
        }

        [HttpGet]
        public async Task<IActionResult> GetRecent()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Json(Array.Empty<object>());

            var notifications = await _notificationService.GetUserNotificationsAsync(user.Id, 10);
            return Json(notifications.Select(n => new
            {
                n.Id,
                n.Title,
                n.Message,
                n.IsRead,
                n.Link,
                Time = n.CreatedDate.ToString("MMM dd, HH:mm")
            }));
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            await _notificationService.MarkAllAsReadAsync(user.Id);
            return Ok();
        }
    }
}
