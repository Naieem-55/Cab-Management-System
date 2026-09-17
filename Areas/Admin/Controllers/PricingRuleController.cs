using CabManagementSystem.Models;
using CabManagementSystem.Models.Enums;
using CabManagementSystem.Models.ViewModels;
using CabManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CabManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public class PricingRuleController : Controller
    {
        private readonly IPricingService _pricingService;
        private readonly ILogger<PricingRuleController> _logger;

        public PricingRuleController(IPricingService pricingService, ILogger<PricingRuleController> logger)
        {
            _pricingService = pricingService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? searchTerm, string? sortOrder, decimal? previewFare, DateTime? previewAt, int page = 1)
        {
            IEnumerable<PricingRule> rules;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                rules = await _pricingService.SearchRulesAsync(searchTerm.Trim());
                ViewBag.SearchTerm = searchTerm;
            }
            else
            {
                rules = await _pricingService.GetAllRulesAsync();
            }

            rules = Helpers.SortHelper.ApplySort(rules, sortOrder);

            var pageSize = 10;
            var paginatedList = PaginatedList<PricingRule>.Create(rules, page, pageSize);

            // Fare preview: what would a trip at this moment cost?
            var fare = previewFare ?? 2500m;
            var at = previewAt ?? DateTime.Now;
            ViewBag.PreviewFare = fare;
            ViewBag.PreviewAt = at;
            ViewBag.PreviewQuote = await _pricingService.GetQuoteAsync(fare, at);

            ViewBag.CurrentSort = sortOrder;
            ViewBag.PageIndex = paginatedList.PageIndex;
            ViewBag.TotalPages = paginatedList.TotalPages;
            ViewBag.TotalCount = paginatedList.TotalCount;
            ViewBag.BaseUrl = Url.Action("Index");

            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(searchTerm)) queryParams.Add($"&searchTerm={searchTerm}");
            if (!string.IsNullOrEmpty(sortOrder)) queryParams.Add($"&sortOrder={sortOrder}");
            ViewBag.QueryString = string.Join("", queryParams);

            return View(paginatedList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new PricingRuleViewModel { SelectedDays = Enum.GetValues<DayOfWeek>().ToList() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PricingRuleViewModel model)
        {
            Validate(model);

            if (ModelState.IsValid)
            {
                try
                {
                    var rule = new PricingRule();
                    model.ApplyTo(rule);
                    await _pricingService.CreateRuleAsync(rule);
                    TempData["SuccessMessage"] = "Pricing rule created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating pricing rule");
                    TempData["ErrorMessage"] = "An unexpected error occurred while creating the pricing rule.";
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var rule = await _pricingService.GetRuleByIdAsync(id);
            if (rule == null)
                return NotFound();

            return View(PricingRuleViewModel.FromEntity(rule));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PricingRuleViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            Validate(model);

            if (ModelState.IsValid)
            {
                try
                {
                    var rule = await _pricingService.GetRuleByIdAsync(id);
                    if (rule == null)
                        return NotFound();

                    model.ApplyTo(rule);
                    await _pricingService.UpdateRuleAsync(rule);
                    TempData["SuccessMessage"] = "Pricing rule updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating pricing rule {Id}", id);
                    TempData["ErrorMessage"] = "An unexpected error occurred while updating the pricing rule.";
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var rule = await _pricingService.GetRuleByIdAsync(id);
            if (rule == null)
                return NotFound();

            return View(rule);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var rule = await _pricingService.GetRuleByIdAsync(id);
            if (rule == null)
                return NotFound();

            return View(rule);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _pricingService.DeleteRuleAsync(id);
                TempData["SuccessMessage"] = "Pricing rule deleted. Existing trips keep their recorded surcharge.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting pricing rule {Id}", id);
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the pricing rule.";
            }
            return RedirectToAction(nameof(Index));
        }

        private void Validate(PricingRuleViewModel model)
        {
            if (model.StartTime == model.EndTime)
                ModelState.AddModelError(nameof(model.EndTime), "End Time must differ from Start Time.");

            if (model.SelectedDays.Count == 0)
                ModelState.AddModelError(nameof(model.SelectedDays), "Select at least one day.");

            if (model.AdjustmentType == DiscountType.Percentage && model.Value > 200)
                ModelState.AddModelError(nameof(model.Value), "Percentage surcharge cannot exceed 200%.");
        }
    }
}
