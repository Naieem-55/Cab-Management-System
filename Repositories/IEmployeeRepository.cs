using CabManagementSystem.Models;
using CabManagementSystem.Models.Enums;

namespace CabManagementSystem.Repositories
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<IEnumerable<Employee>> GetEmployeesByPositionAsync(EmployeePosition position);
        Task<IEnumerable<Employee>> SearchEmployeesAsync(string searchTerm);
        Task<Employee?> GetEmployeeWithDriverAsync(int id);
    }
}
