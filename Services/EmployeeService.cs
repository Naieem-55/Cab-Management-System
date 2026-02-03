using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Cab_Management_System.Repositories;

namespace Cab_Management_System.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
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
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            _employeeRepository.Update(employee);
            await _employeeRepository.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                throw new KeyNotFoundException($"Employee with ID {id} not found.");

            _employeeRepository.Remove(employee);
            await _employeeRepository.SaveChangesAsync();
        }

        public async Task<int> GetEmployeeCountAsync()
        {
            return await _employeeRepository.CountAsync();
        }

        public async Task<int> GetActiveEmployeeCountAsync()
        {
            return await _employeeRepository.CountAsync(e => e.Status == EmployeeStatus.Active);
        }
    }
}
