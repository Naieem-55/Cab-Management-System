using Cab_Management_System.Models;
using Cab_Management_System.Repositories;
using Microsoft.Extensions.Logging;

namespace Cab_Management_System.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ITripRepository _tripRepository;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(ICustomerRepository customerRepository, ITripRepository tripRepository, ILogger<CustomerService> logger)
        {
            _customerRepository = customerRepository;
            _tripRepository = tripRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
            => await _customerRepository.GetAllAsync();

        public async Task<Customer?> GetCustomerByIdAsync(int id)
            => await _customerRepository.GetByIdAsync(id);

        public async Task<Customer?> GetCustomerWithTripsAsync(int id)
            => await _customerRepository.GetCustomerWithTripsAsync(id);

        public async Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm)
            => await _customerRepository.SearchCustomersAsync(searchTerm);

        public async Task CreateCustomerAsync(Customer customer)
        {
            await _customerRepository.AddAsync(customer);
            await _customerRepository.SaveChangesAsync();
            _logger.LogInformation("Created Customer with ID {Id}", customer.Id);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            _customerRepository.Update(customer);
            await _customerRepository.SaveChangesAsync();
            _logger.LogInformation("Updated Customer with ID {Id}", customer.Id);
        }

        public async Task DeleteCustomerAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                _logger.LogWarning("Customer with ID {Id} not found", id);
                throw new KeyNotFoundException($"Customer with ID {id} not found.");
            }

            _customerRepository.Remove(customer);
            await _customerRepository.SaveChangesAsync();
            _logger.LogInformation("Deleted Customer with ID {Id}", id);
        }

        public async Task<bool> CanDeleteAsync(int id)
        {
            var tripCount = await _tripRepository.CountAsync(t => t.CustomerId == id);
            return tripCount == 0;
        }

        public async Task<int> GetCustomerCountAsync()
            => await _customerRepository.CountAsync();
    }
}
