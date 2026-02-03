using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;

namespace Cab_Management_System.Repositories
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<IEnumerable<Employee>> GetEmployeesByPositionAsync(EmployeePosition position);
        Task<IEnumerable<Employee>> SearchEmployeesAsync(string searchTerm);
        Task<Employee?> GetEmployeeWithDriverAsync(int id);
    }
}
