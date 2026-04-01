using CabManagementSystem.Models;
using CabManagementSystem.Models.Enums;

namespace CabManagementSystem.Services
{
    public interface IExpenseService
    {
        Task<IEnumerable<Expense>> GetAllExpensesAsync();
        Task<Expense?> GetExpenseByIdAsync(int id);
        Task<Expense?> GetExpenseWithDetailsAsync(int id);
        Task<IEnumerable<Expense>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Expense>> GetByCategoryAsync(ExpenseCategory category);
        Task<IEnumerable<Expense>> GetByVehicleAsync(int vehicleId);
        Task CreateExpenseAsync(Expense expense);
        Task UpdateExpenseAsync(Expense expense);
        Task DeleteExpenseAsync(int id);
        Task<decimal> GetTotalExpensesAsync();
        Task<decimal> GetTotalExpensesByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
