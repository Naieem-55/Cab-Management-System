using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Cab_Management_System.Models.ViewModels;
using Cab_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Cab_Management_System.Areas.Finance.Controllers
{
    [Area("Finance")]
    [Authorize(Roles = "FinanceManager")]
    public class BillingController : Controller
    {
        private readonly IBillingService _billingService;
        private readonly ITripService _tripService;
        private readonly IInvoicePdfService _invoicePdfService;
        private readonly ILogger<BillingController> _logger;

        public BillingController(IBillingService billingService, ITripService tripService, IInvoicePdfService invoicePdfService, ILogger<BillingController> logger)
        {
            _billingService = billingService;
            _tripService = tripService;
            _invoicePdfService = invoicePdfService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? searchTerm, PaymentStatus? status, DateTime? startDate, DateTime? endDate, int page = 1)
        {
            IEnumerable<Billing> billings;

            if (startDate.HasValue && endDate.HasValue)
            {
                billings = await _billingService.GetBillingsByDateRangeAsync(startDate.Value, endDate.Value);
            }
            else if (status.HasValue)
            {
                billings = await _billingService.GetBillingsByStatusAsync(status.Value);
            }
            else
            {
                billings = await _billingService.GetAllBillingsAsync();
            }

            if (status.HasValue && startDate.HasValue && endDate.HasValue)
            {
                billings = billings.Where(b => b.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var matchingTrips = await _tripService.SearchTripsAsync(searchTerm);
                var matchingTripIds = matchingTrips.Select(t => t.Id).ToHashSet();
                billings = billings.Where(b => matchingTripIds.Contains(b.TripId));
            }

            var pageSize = 10;
            var paginatedList = PaginatedList<Billing>.Create(billings, page, pageSize);

            ViewBag.Statuses = new SelectList(Enum.GetValues<PaymentStatus>());
            ViewData["SearchTerm"] = searchTerm;
            ViewData["SelectedStatus"] = status;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");
            ViewBag.PageIndex = paginatedList.PageIndex;
            ViewBag.TotalPages = paginatedList.TotalPages;
            ViewBag.TotalCount = paginatedList.TotalCount;
            ViewBag.BaseUrl = Url.Action("Index");

            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(searchTerm)) queryParams.Add($"&searchTerm={searchTerm}");
            if (status.HasValue) queryParams.Add($"&status={status.Value}");
            if (startDate.HasValue) queryParams.Add($"&startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue) queryParams.Add($"&endDate={endDate.Value:yyyy-MM-dd}");
            ViewBag.QueryString = string.Join("", queryParams);

            return View(paginatedList);
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
                try
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
                    TempData["SuccessMessage"] = "Billing record created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error creating billing");
                    TempData["ErrorMessage"] = "A database error occurred while creating the billing record.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating billing");
                    TempData["ErrorMessage"] = "An unexpected error occurred while creating the billing record.";
                }
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
                try
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
                    TempData["SuccessMessage"] = "Billing record updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error updating billing {Id}", id);
                    TempData["ErrorMessage"] = "A database error occurred while updating the billing record.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating billing {Id}", id);
                    TempData["ErrorMessage"] = "An unexpected error occurred while updating the billing record.";
                }
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
            try
            {
                await _billingService.DeleteBillingAsync(id);
                TempData["SuccessMessage"] = "Billing record deleted successfully.";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error deleting billing {Id}", id);
                TempData["ErrorMessage"] = "Cannot delete this billing record because it has related records.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting billing {Id}", id);
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the billing record.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GenerateInvoice(int id)
        {
            var billing = await _billingService.GetBillingWithTripAsync(id);
            if (billing == null)
                return NotFound();

            try
            {
                var pdf = _invoicePdfService.GenerateInvoice(billing);
                return File(pdf, "application/pdf", $"Invoice_INV-{billing.Id:D5}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating invoice for billing {Id}", id);
                TempData["ErrorMessage"] = "An error occurred while generating the invoice.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        public async Task<IActionResult> Export()
        {
            var billings = await _billingService.GetAllBillingsAsync();
            var columns = new Dictionary<string, Func<Billing, object>>
            {
                { "Trip #", b => b.TripId },
                { "Customer", b => b.Trip?.CustomerName ?? "" },
                { "Amount", b => b.Amount },
                { "Payment Date", b => b.PaymentDate.ToString("yyyy-MM-dd") },
                { "Payment Method", b => b.PaymentMethod },
                { "Status", b => b.Status }
            };

            var csvData = Helpers.CsvExportHelper.ExportToCsv(billings, columns);
            return File(csvData, "text/csv", $"Billings_{DateTime.Now:yyyyMMdd}.csv");
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
