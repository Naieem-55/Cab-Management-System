using Cab_Management_System.Models;

namespace Cab_Management_System.Repositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetCustomerWithTripsAsync(int id);
        Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm);
    }
}
