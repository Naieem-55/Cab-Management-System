using CabManagementSystem.Models;
using CabManagementSystem.Models.Enums;

namespace CabManagementSystem.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<Employee?> GetEmployeeWithDriverAsync(int id);
        Task<IEnumerable<Employee>> GetEmployeesByPositionAsync(EmployeePosition position);
        Task<IEnumerable<Employee>> SearchEmployeesAsync(string searchTerm);
        Task CreateEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(int id);
        Task<int> GetEmployeeCountAsync();
        Task<int> GetActiveEmployeeCountAsync();
        Task<bool> CanDeleteAsync(int id);
        Task<bool> HasDriverRecordAsync(int id);
    }
}
