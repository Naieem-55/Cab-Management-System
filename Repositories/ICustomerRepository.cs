using CabManagementSystem.Models;

namespace CabManagementSystem.Repositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetCustomerWithTripsAsync(int id);
        Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm);
    }
}
