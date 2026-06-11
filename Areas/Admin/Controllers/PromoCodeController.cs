using CabManagementSystem.Models;
using CabManagementSystem.Models.Enums;
using CabManagementSystem.Models.ViewModels;
using CabManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CabManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public class PromoCodeController : Controller
    {
        private readonly IPromoCodeService _promoCodeService;
        private readonly ILogger<PromoCodeController> _logger;

        public PromoCodeController(IPromoCodeService promoCodeService, ILogger<PromoCodeController> logger)
        {
            _promoCodeService = promoCodeService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? searchTerm, int page = 1)
        {
            IEnumerable<PromoCode> promoCodes;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                promoCodes = await _promoCodeService.SearchPromoCodesAsync(searchTerm.Trim());
                ViewBag.SearchTerm = searchTerm;
            }
            else
            {
                promoCodes = await _promoCodeService.GetAllPromoCodesAsync();
            }

            var pageSize = 10;
            var paginatedList = PaginatedList<PromoCode>.Create(promoCodes, page, pageSize);

            ViewBag.PageIndex = paginatedList.PageIndex;
            ViewBag.TotalPages = paginatedList.TotalPages;
            ViewBag.TotalCount = paginatedList.TotalCount;
            ViewBag.BaseUrl = Url.Action("Index");

            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(searchTerm)) queryParams.Add($"&searchTerm={searchTerm}");
            ViewBag.QueryString = string.Join("", queryParams);

            return View(paginatedList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new PromoCodeViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PromoCodeViewModel model)
        {
            if (model.ValidUntil <= model.ValidFrom)
                ModelState.AddModelError(nameof(model.ValidUntil), "Valid Until must be after Valid From.");

            if (ModelState.IsValid)
            {
                try
                {
                    var promoCode = new PromoCode
                    {
                        Code = model.Code,
                        Description = model.Description,
                        DiscountType = model.DiscountType,
                        DiscountValue = model.DiscountValue,
                        MaxDiscountAmount = model.MaxDiscountAmount,
                        MinTripCost = model.MinTripCost,
                        UsageLimit = model.UsageLimit,
                        ValidFrom = model.ValidFrom,
                        ValidUntil = model.ValidUntil,
                        IsActive = model.IsActive
                    };

                    await _promoCodeService.CreatePromoCodeAsync(promoCode);
                    TempData["SuccessMessage"] = "Promo code created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error creating promo code");
                    if (ex.InnerException?.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) == true ||
                        ex.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        ModelState.AddModelError("Code", "A promo code with this code already exists.");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "A database error occurred while creating the promo code.";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating promo code");
                    TempData["ErrorMessage"] = "An unexpected error occurred while creating the promo code.";
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var promoCode = await _promoCodeService.GetPromoCodeByIdAsync(id);
            if (promoCode == null)
                return NotFound();

            var model = new PromoCodeViewModel
            {
                Id = promoCode.Id,
                Code = promoCode.Code,
                Description = promoCode.Description,
                DiscountType = promoCode.DiscountType,
                DiscountValue = promoCode.DiscountValue,
                MaxDiscountAmount = promoCode.MaxDiscountAmount,
                MinTripCost = promoCode.MinTripCost,
                UsageLimit = promoCode.UsageLimit,
                TimesUsed = promoCode.TimesUsed,
                ValidFrom = promoCode.ValidFrom,
                ValidUntil = promoCode.ValidUntil,
                IsActive = promoCode.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PromoCodeViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (model.ValidUntil <= model.ValidFrom)
                ModelState.AddModelError(nameof(model.ValidUntil), "Valid Until must be after Valid From.");

            if (ModelState.IsValid)
            {
                try
                {
                    var promoCode = await _promoCodeService.GetPromoCodeByIdAsync(id);
                    if (promoCode == null)
                        return NotFound();

                    promoCode.Code = model.Code;
                    promoCode.Description = model.Description;
                    promoCode.DiscountType = model.DiscountType;
                    promoCode.DiscountValue = model.DiscountValue;
                    promoCode.MaxDiscountAmount = model.MaxDiscountAmount;
                    promoCode.MinTripCost = model.MinTripCost;
                    promoCode.UsageLimit = model.UsageLimit;
                    promoCode.ValidFrom = model.ValidFrom;
                    promoCode.ValidUntil = model.ValidUntil;
                    promoCode.IsActive = model.IsActive;

                    await _promoCodeService.UpdatePromoCodeAsync(promoCode);
                    TempData["SuccessMessage"] = "Promo code updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error updating promo code {Id}", id);
                    if (ex.InnerException?.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) == true ||
                        ex.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        ModelState.AddModelError("Code", "A promo code with this code already exists.");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "A database error occurred while updating the promo code.";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating promo code {Id}", id);
                    TempData["ErrorMessage"] = "An unexpected error occurred while updating the promo code.";
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var promoCode = await _promoCodeService.GetPromoCodeByIdAsync(id);
            if (promoCode == null)
                return NotFound();

            return View(promoCode);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var promoCode = await _promoCodeService.GetPromoCodeByIdAsync(id);
            if (promoCode == null)
                return NotFound();

            return View(promoCode);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _promoCodeService.DeletePromoCodeAsync(id);
                TempData["SuccessMessage"] = "Promo code deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting promo code {Id}", id);
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the promo code.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
