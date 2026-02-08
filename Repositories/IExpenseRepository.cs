using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;

namespace Cab_Management_System.Repositories
{
    public interface IExpenseRepository : IRepository<Expense>
    {
        Task<Expense?> GetExpenseWithDetailsAsync(int id);
        Task<IEnumerable<Expense>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Expense>> GetByCategoryAsync(ExpenseCategory category);
        Task<IEnumerable<Expense>> GetByVehicleAsync(int vehicleId);
        Task<decimal> GetTotalExpensesAsync();
        Task<decimal> GetTotalExpensesByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
