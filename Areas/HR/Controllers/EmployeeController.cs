using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Cab_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Cab_Management_System.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize(Roles = "HRManager")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(IEmployeeService employeeService, ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? searchTerm, EmployeePosition? position, EmployeeStatus? status, int page = 1)
        {
            IEnumerable<Employee> employees;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                employees = await _employeeService.SearchEmployeesAsync(searchTerm.Trim());
            }
            else
            {
                employees = await _employeeService.GetAllEmployeesAsync();
            }

            if (position.HasValue)
            {
                employees = employees.Where(e => e.Position == position.Value);
            }

            if (status.HasValue)
            {
                employees = employees.Where(e => e.Status == status.Value);
            }

            var pageSize = 10;
            var paginatedList = PaginatedList<Employee>.Create(employees, page, pageSize);

            ViewBag.Positions = new SelectList(Enum.GetValues<EmployeePosition>());
            ViewBag.Statuses = new SelectList(Enum.GetValues<EmployeeStatus>());
            ViewData["SearchTerm"] = searchTerm;
            ViewData["SelectedPosition"] = position;
            ViewData["SelectedStatus"] = status;
            ViewBag.PageIndex = paginatedList.PageIndex;
            ViewBag.TotalPages = paginatedList.TotalPages;
            ViewBag.TotalCount = paginatedList.TotalCount;
            ViewBag.BaseUrl = Url.Action("Index");

            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(searchTerm)) queryParams.Add($"&searchTerm={searchTerm}");
            if (position.HasValue) queryParams.Add($"&position={position.Value}");
            if (status.HasValue) queryParams.Add($"&status={status.Value}");
            ViewBag.QueryString = string.Join("", queryParams);

            return View(paginatedList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Employee { HireDate = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _employeeService.CreateEmployeeAsync(employee);
                    TempData["SuccessMessage"] = "Employee created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error creating employee");
                    if (ex.InnerException?.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) == true ||
                        ex.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        ModelState.AddModelError("Email", "An employee with this email already exists.");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "A database error occurred while creating the employee.";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating employee");
                    TempData["ErrorMessage"] = "An unexpected error occurred while creating the employee.";
                }
            }
            return View(employee);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound();

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _employeeService.UpdateEmployeeAsync(employee);
                    TempData["SuccessMessage"] = "Employee updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error updating employee {Id}", id);
                    if (ex.InnerException?.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) == true ||
                        ex.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        ModelState.AddModelError("Email", "An employee with this email already exists.");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "A database error occurred while updating the employee.";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating employee {Id}", id);
                    TempData["ErrorMessage"] = "An unexpected error occurred while updating the employee.";
                }
            }
            return View(employee);
        }

        public async Task<IActionResult> Details(int id)
        {
            var employee = await _employeeService.GetEmployeeWithDriverAsync(id);
            if (employee == null)
                return NotFound();

            return View(employee);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound();

            var hasDriver = await _employeeService.HasDriverRecordAsync(id);
            ViewBag.HasDriverRecord = hasDriver;

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var canDelete = await _employeeService.CanDeleteAsync(id);
                if (!canDelete)
                {
                    TempData["ErrorMessage"] = "Cannot delete this employee because they have an associated driver record. Please remove the driver record first.";
                    return RedirectToAction(nameof(Index));
                }

                await _employeeService.DeleteEmployeeAsync(id);
                TempData["SuccessMessage"] = "Employee deleted successfully.";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error deleting employee {Id}", id);
                TempData["ErrorMessage"] = "Cannot delete this employee because they have related records.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee {Id}", id);
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the employee.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Export()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            var columns = new Dictionary<string, Func<Employee, object>>
            {
                { "Name", e => e.Name },
                { "Email", e => e.Email },
                { "Phone", e => e.Phone },
                { "Position", e => e.Position },
                { "Status", e => e.Status },
                { "Hire Date", e => e.HireDate.ToString("yyyy-MM-dd") },
                { "Salary", e => e.Salary }
            };

            var csvData = Helpers.CsvExportHelper.ExportToCsv(employees, columns);
            return File(csvData, "text/csv", $"Employees_{DateTime.Now:yyyyMMdd}.csv");
        }
    }
}
