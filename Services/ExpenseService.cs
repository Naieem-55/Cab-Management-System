using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Cab_Management_System.Repositories;
using Microsoft.Extensions.Logging;

namespace Cab_Management_System.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly ILogger<ExpenseService> _logger;

        public ExpenseService(IExpenseRepository expenseRepository, ILogger<ExpenseService> logger)
        {
            _expenseRepository = expenseRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Expense>> GetAllExpensesAsync()
            => await _expenseRepository.GetAllAsync();

        public async Task<Expense?> GetExpenseByIdAsync(int id)
            => await _expenseRepository.GetByIdAsync(id);

        public async Task<Expense?> GetExpenseWithDetailsAsync(int id)
            => await _expenseRepository.GetExpenseWithDetailsAsync(id);

        public async Task<IEnumerable<Expense>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
            => await _expenseRepository.GetByDateRangeAsync(startDate, endDate);

        public async Task<IEnumerable<Expense>> GetByCategoryAsync(ExpenseCategory category)
            => await _expenseRepository.GetByCategoryAsync(category);

        public async Task<IEnumerable<Expense>> GetByVehicleAsync(int vehicleId)
            => await _expenseRepository.GetByVehicleAsync(vehicleId);

        public async Task CreateExpenseAsync(Expense expense)
        {
            await _expenseRepository.AddAsync(expense);
            await _expenseRepository.SaveChangesAsync();
            _logger.LogInformation("Created Expense with ID {Id}", expense.Id);
        }

        public async Task UpdateExpenseAsync(Expense expense)
        {
            _expenseRepository.Update(expense);
            await _expenseRepository.SaveChangesAsync();
            _logger.LogInformation("Updated Expense with ID {Id}", expense.Id);
        }

        public async Task DeleteExpenseAsync(int id)
        {
            var expense = await _expenseRepository.GetByIdAsync(id);
            if (expense == null)
            {
                _logger.LogWarning("Expense with ID {Id} not found", id);
                throw new KeyNotFoundException($"Expense with ID {id} not found.");
            }

            _expenseRepository.Remove(expense);
            await _expenseRepository.SaveChangesAsync();
            _logger.LogInformation("Deleted Expense with ID {Id}", id);
        }

        public async Task<decimal> GetTotalExpensesAsync()
            => await _expenseRepository.GetTotalExpensesAsync();

        public async Task<decimal> GetTotalExpensesByDateRangeAsync(DateTime startDate, DateTime endDate)
            => await _expenseRepository.GetTotalExpensesByDateRangeAsync(startDate, endDate);
    }
}
