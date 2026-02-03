using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;

namespace Cab_Management_System.Services
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
    }
}
