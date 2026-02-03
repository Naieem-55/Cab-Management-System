using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Cab_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cab_Management_System.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize(Roles = "HRManager")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<IActionResult> Index(string? searchTerm, EmployeePosition? position, EmployeeStatus? status)
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

            ViewBag.Positions = new SelectList(Enum.GetValues<EmployeePosition>());
            ViewBag.Statuses = new SelectList(Enum.GetValues<EmployeeStatus>());
            ViewData["SearchTerm"] = searchTerm;
            ViewData["SelectedPosition"] = position;
            ViewData["SelectedStatus"] = status;

            return View(employees);
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
                await _employeeService.CreateEmployeeAsync(employee);
                TempData["SuccessMessage"] = "Employee created successfully.";
                return RedirectToAction(nameof(Index));
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
                await _employeeService.UpdateEmployeeAsync(employee);
                TempData["SuccessMessage"] = "Employee updated successfully.";
                return RedirectToAction(nameof(Index));
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

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _employeeService.DeleteEmployeeAsync(id);
            TempData["SuccessMessage"] = "Employee deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
