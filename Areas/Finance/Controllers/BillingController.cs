using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Cab_Management_System.Models.ViewModels;
using Cab_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cab_Management_System.Areas.Finance.Controllers
{
    [Area("Finance")]
    [Authorize(Roles = "FinanceManager")]
    public class BillingController : Controller
    {
        private readonly IBillingService _billingService;
        private readonly ITripService _tripService;

        public BillingController(IBillingService billingService, ITripService tripService)
        {
            _billingService = billingService;
            _tripService = tripService;
        }

        public async Task<IActionResult> Index(string? searchTerm, PaymentStatus? status)
        {
            var billings = status.HasValue
                ? await _billingService.GetBillingsByStatusAsync(status.Value)
                : await _billingService.GetAllBillingsAsync();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var matchingTrips = await _tripService.SearchTripsAsync(searchTerm);
                var matchingTripIds = matchingTrips.Select(t => t.Id).ToHashSet();
                billings = billings.Where(b => matchingTripIds.Contains(b.TripId));
            }

            ViewBag.Statuses = new SelectList(Enum.GetValues<PaymentStatus>());
            ViewData["SearchTerm"] = searchTerm;
            ViewData["SelectedStatus"] = status;

            return View(billings);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new BillingViewModel
            {
                PaymentDate = DateTime.Now,
                AvailableTrips = await GetAvailableTripsSelectList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BillingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var billing = new Billing
                {
                    TripId = model.TripId,
                    Amount = model.Amount,
                    PaymentDate = model.PaymentDate,
                    PaymentMethod = model.PaymentMethod,
                    Status = model.Status
                };

                await _billingService.CreateBillingAsync(billing);
                return RedirectToAction(nameof(Index));
            }

            model.AvailableTrips = await GetAvailableTripsSelectList();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var billing = await _billingService.GetBillingByIdAsync(id);
            if (billing == null)
            {
                return NotFound();
            }

            var model = new BillingViewModel
            {
                Id = billing.Id,
                TripId = billing.TripId,
                Amount = billing.Amount,
                PaymentDate = billing.PaymentDate,
                PaymentMethod = billing.PaymentMethod,
                Status = billing.Status,
                AvailableTrips = await GetAvailableTripsSelectList(billing.TripId)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BillingViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var billing = await _billingService.GetBillingByIdAsync(id);
                if (billing == null)
                {
                    return NotFound();
                }

                billing.TripId = model.TripId;
                billing.Amount = model.Amount;
                billing.PaymentDate = model.PaymentDate;
                billing.PaymentMethod = model.PaymentMethod;
                billing.Status = model.Status;

                await _billingService.UpdateBillingAsync(billing);
                return RedirectToAction(nameof(Index));
            }

            model.AvailableTrips = await GetAvailableTripsSelectList(model.TripId);
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var billing = await _billingService.GetBillingWithTripAsync(id);
            if (billing == null)
            {
                return NotFound();
            }

            return View(billing);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var billing = await _billingService.GetBillingWithTripAsync(id);
            if (billing == null)
            {
                return NotFound();
            }

            return View(billing);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _billingService.DeleteBillingAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<SelectList> GetAvailableTripsSelectList(int? currentTripId = null)
        {
            var allTrips = await _tripService.GetAllTripsAsync();
            var availableTrips = allTrips.Where(t => t.Billing == null || t.Id == currentTripId);
            return new SelectList(
                availableTrips.Select(t => new { t.Id, Display = $"Trip #{t.Id} - {t.CustomerName}" }),
                "Id",
                "Display",
                currentTripId
            );
        }
    }
}
