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
    public class ExpenseController : Controller
    {
        private readonly IExpenseService _expenseService;
        private readonly IVehicleService _vehicleService;
        private readonly ITripService _tripService;
        private readonly ILogger<ExpenseController> _logger;

        public ExpenseController(
            IExpenseService expenseService,
            IVehicleService vehicleService,
            ITripService tripService,
            ILogger<ExpenseController> logger)
        {
            _expenseService = expenseService;
            _vehicleService = vehicleService;
            _tripService = tripService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(ExpenseCategory? category, int? vehicleId, DateTime? startDate, DateTime? endDate, int page = 1)
        {
            IEnumerable<Expense> expenses;

            if (startDate.HasValue && endDate.HasValue)
            {
                expenses = await _expenseService.GetByDateRangeAsync(startDate.Value, endDate.Value);
            }
            else
            {
                expenses = await _expenseService.GetAllExpensesAsync();
            }

            if (category.HasValue)
            {
                expenses = expenses.Where(e => e.Category == category.Value);
            }

            if (vehicleId.HasValue)
            {
                expenses = expenses.Where(e => e.VehicleId == vehicleId.Value);
            }

            var pageSize = 10;
            var paginatedList = PaginatedList<Expense>.Create(expenses, page, pageSize);

            ViewBag.Categories = new SelectList(Enum.GetValues<ExpenseCategory>());
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            ViewBag.Vehicles = new SelectList(vehicles, "Id", "RegistrationNumber");
            ViewData["SelectedCategory"] = category;
            ViewData["SelectedVehicle"] = vehicleId;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");
            ViewBag.PageIndex = paginatedList.PageIndex;
            ViewBag.TotalPages = paginatedList.TotalPages;
            ViewBag.TotalCount = paginatedList.TotalCount;
            ViewBag.BaseUrl = Url.Action("Index");

            var queryParams = new List<string>();
            if (category.HasValue) queryParams.Add($"&category={category.Value}");
            if (vehicleId.HasValue) queryParams.Add($"&vehicleId={vehicleId.Value}");
            if (startDate.HasValue) queryParams.Add($"&startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue) queryParams.Add($"&endDate={endDate.Value:yyyy-MM-dd}");
            ViewBag.QueryString = string.Join("", queryParams);

            return View(paginatedList);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new ExpenseViewModel { Date = DateTime.Now };
            await PopulateDropdownsAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpenseViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var expense = new Expense
                    {
                        VehicleId = model.VehicleId,
                        TripId = model.TripId,
                        Category = model.Category,
                        Amount = model.Amount,
                        Date = model.Date,
                        Description = model.Description,
                        ApprovedBy = model.ApprovedBy
                    };

                    await _expenseService.CreateExpenseAsync(expense);
                    TempData["SuccessMessage"] = "Expense recorded successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error creating expense");
                    TempData["ErrorMessage"] = "A database error occurred while creating the expense.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating expense");
                    TempData["ErrorMessage"] = "An unexpected error occurred while creating the expense.";
                }
            }

            await PopulateDropdownsAsync(model);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var expense = await _expenseService.GetExpenseWithDetailsAsync(id);
            if (expense == null)
                return NotFound();

            var model = new ExpenseViewModel
            {
                Id = expense.Id,
                VehicleId = expense.VehicleId,
                TripId = expense.TripId,
                Category = expense.Category,
                Amount = expense.Amount,
                Date = expense.Date,
                Description = expense.Description,
                ApprovedBy = expense.ApprovedBy
            };

            await PopulateDropdownsAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExpenseViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var expense = await _expenseService.GetExpenseByIdAsync(id);
                    if (expense == null)
                        return NotFound();

                    expense.VehicleId = model.VehicleId;
                    expense.TripId = model.TripId;
                    expense.Category = model.Category;
                    expense.Amount = model.Amount;
                    expense.Date = model.Date;
                    expense.Description = model.Description;
                    expense.ApprovedBy = model.ApprovedBy;

                    await _expenseService.UpdateExpenseAsync(expense);
                    TempData["SuccessMessage"] = "Expense updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error updating expense {Id}", id);
                    TempData["ErrorMessage"] = "A database error occurred while updating the expense.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating expense {Id}", id);
                    TempData["ErrorMessage"] = "An unexpected error occurred while updating the expense.";
                }
            }

            await PopulateDropdownsAsync(model);
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var expense = await _expenseService.GetExpenseWithDetailsAsync(id);
            if (expense == null)
                return NotFound();

            return View(expense);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var expense = await _expenseService.GetExpenseWithDetailsAsync(id);
            if (expense == null)
                return NotFound();

            return View(expense);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _expenseService.DeleteExpenseAsync(id);
                TempData["SuccessMessage"] = "Expense deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting expense {Id}", id);
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the expense.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Export()
        {
            var expenses = await _expenseService.GetAllExpensesAsync();
            var columns = new Dictionary<string, Func<Expense, object>>
            {
                { "Category", e => e.Category },
                { "Amount", e => e.Amount },
                { "Date", e => e.Date.ToString("yyyy-MM-dd") },
                { "Vehicle", e => e.Vehicle?.RegistrationNumber ?? "" },
                { "Trip #", e => e.TripId?.ToString() ?? "" },
                { "Description", e => e.Description ?? "" },
                { "Approved By", e => e.ApprovedBy ?? "" }
            };

            var csvData = Helpers.CsvExportHelper.ExportToCsv(expenses, columns);
            return File(csvData, "text/csv", $"Expenses_{DateTime.Now:yyyyMMdd}.csv");
        }

        private async Task PopulateDropdownsAsync(ExpenseViewModel model)
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            model.AvailableVehicles = new SelectList(vehicles, "Id", "RegistrationNumber");

            var trips = await _tripService.GetAllTripsAsync();
            model.AvailableTrips = new SelectList(
                trips.Select(t => new { t.Id, Display = $"Trip #{t.Id} - {t.CustomerName}" }),
                "Id", "Display");
        }
    }
}
