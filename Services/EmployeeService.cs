using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Cab_Management_System.Repositories;
using Microsoft.Extensions.Logging;

namespace Cab_Management_System.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(IEmployeeRepository employeeRepository, IDriverRepository driverRepository, ILogger<EmployeeService> logger)
        {
            _employeeRepository = employeeRepository;
            _driverRepository = driverRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _employeeRepository.GetAllAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _employeeRepository.GetByIdAsync(id);
        }

        public async Task<Employee?> GetEmployeeWithDriverAsync(int id)
        {
            return await _employeeRepository.GetEmployeeWithDriverAsync(id);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByPositionAsync(EmployeePosition position)
        {
            return await _employeeRepository.GetEmployeesByPositionAsync(position);
        }

        public async Task<IEnumerable<Employee>> SearchEmployeesAsync(string searchTerm)
        {
            return await _employeeRepository.SearchEmployeesAsync(searchTerm);
        }

        public async Task CreateEmployeeAsync(Employee employee)
        {
            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.SaveChangesAsync();
            _logger.LogInformation("Created Employee with ID {Id}", employee.Id);
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            _employeeRepository.Update(employee);
            await _employeeRepository.SaveChangesAsync();
            _logger.LogInformation("Updated Employee with ID {Id}", employee.Id);
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                _logger.LogWarning("Employee with ID {Id} not found", id);
                throw new KeyNotFoundException($"Employee with ID {id} not found.");
            }

            _employeeRepository.Remove(employee);
            await _employeeRepository.SaveChangesAsync();
            _logger.LogInformation("Deleted Employee with ID {Id}", id);
        }

        public async Task<int> GetEmployeeCountAsync()
        {
            return await _employeeRepository.CountAsync();
        }

        public async Task<int> GetActiveEmployeeCountAsync()
        {
            return await _employeeRepository.CountAsync(e => e.Status == EmployeeStatus.Active);
        }

        public async Task<bool> CanDeleteAsync(int id)
        {
            var driverCount = await _driverRepository.CountAsync(d => d.EmployeeId == id);
            return driverCount == 0;
        }

        public async Task<bool> HasDriverRecordAsync(int id)
        {
            var driverCount = await _driverRepository.CountAsync(d => d.EmployeeId == id);
            return driverCount > 0;
        }
    }
}
