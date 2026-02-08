using Cab_Management_System.Models;
using Cab_Management_System.Models.ViewModels;
using Cab_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cab_Management_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? searchTerm, int page = 1)
        {
            IEnumerable<Customer> customers;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                customers = await _customerService.SearchCustomersAsync(searchTerm.Trim());
                ViewBag.SearchTerm = searchTerm;
            }
            else
            {
                customers = await _customerService.GetAllCustomersAsync();
            }

            var pageSize = 10;
            var paginatedList = PaginatedList<Customer>.Create(customers, page, pageSize);

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
            return View(new CustomerViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var customer = new Customer
                    {
                        Name = model.Name,
                        Email = model.Email,
                        Phone = model.Phone,
                        Address = model.Address
                    };

                    await _customerService.CreateCustomerAsync(customer);
                    TempData["SuccessMessage"] = "Customer created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error creating customer");
                    if (ex.InnerException?.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) == true ||
                        ex.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        ModelState.AddModelError("Email", "A customer with this email already exists.");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "A database error occurred while creating the customer.";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating customer");
                    TempData["ErrorMessage"] = "An unexpected error occurred while creating the customer.";
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return NotFound();

            var model = new CustomerViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CustomerViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var customer = await _customerService.GetCustomerByIdAsync(id);
                    if (customer == null)
                        return NotFound();

                    customer.Name = model.Name;
                    customer.Email = model.Email;
                    customer.Phone = model.Phone;
                    customer.Address = model.Address;

                    await _customerService.UpdateCustomerAsync(customer);
                    TempData["SuccessMessage"] = "Customer updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error updating customer {Id}", id);
                    if (ex.InnerException?.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) == true ||
                        ex.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        ModelState.AddModelError("Email", "A customer with this email already exists.");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "A database error occurred while updating the customer.";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating customer {Id}", id);
                    TempData["ErrorMessage"] = "An unexpected error occurred while updating the customer.";
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var customer = await _customerService.GetCustomerWithTripsAsync(id);
            if (customer == null)
                return NotFound();

            return View(customer);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _customerService.GetCustomerWithTripsAsync(id);
            if (customer == null)
                return NotFound();

            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var canDelete = await _customerService.CanDeleteAsync(id);
                if (!canDelete)
                {
                    TempData["ErrorMessage"] = "Cannot delete this customer because they have associated trips.";
                    return RedirectToAction(nameof(Index));
                }

                await _customerService.DeleteCustomerAsync(id);
                TempData["SuccessMessage"] = "Customer deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer {Id}", id);
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the customer.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Export()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            var columns = new Dictionary<string, Func<Customer, object>>
            {
                { "Name", c => c.Name },
                { "Email", c => c.Email },
                { "Phone", c => c.Phone },
                { "Address", c => c.Address ?? "" },
                { "Total Trips", c => c.Trips?.Count ?? 0 }
            };

            var csvData = Helpers.CsvExportHelper.ExportToCsv(customers, columns);
            return File(csvData, "text/csv", $"Customers_{DateTime.Now:yyyyMMdd}.csv");
        }
    }
}
