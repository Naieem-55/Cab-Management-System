using Cab_Management_System.Data;
using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Cab_Management_System.Repositories
{
    public class ExpenseRepository : Repository<Expense>, IExpenseRepository
    {
        public ExpenseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Expense>> GetAllAsync()
            => await _dbSet.Include(e => e.Vehicle)
                           .Include(e => e.Trip)
                           .OrderByDescending(e => e.Date)
                           .ToListAsync();

        public async Task<Expense?> GetExpenseWithDetailsAsync(int id)
            => await _dbSet.Include(e => e.Vehicle)
                           .Include(e => e.Trip)
                           .FirstOrDefaultAsync(e => e.Id == id);

        public async Task<IEnumerable<Expense>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
            => await _dbSet.Include(e => e.Vehicle)
                           .Include(e => e.Trip)
                           .Where(e => e.Date >= startDate && e.Date <= endDate)
                           .OrderByDescending(e => e.Date)
                           .ToListAsync();

        public async Task<IEnumerable<Expense>> GetByCategoryAsync(ExpenseCategory category)
            => await _dbSet.Include(e => e.Vehicle)
                           .Include(e => e.Trip)
                           .Where(e => e.Category == category)
                           .OrderByDescending(e => e.Date)
                           .ToListAsync();

        public async Task<IEnumerable<Expense>> GetByVehicleAsync(int vehicleId)
            => await _dbSet.Include(e => e.Vehicle)
                           .Include(e => e.Trip)
                           .Where(e => e.VehicleId == vehicleId)
                           .OrderByDescending(e => e.Date)
                           .ToListAsync();

        public async Task<decimal> GetTotalExpensesAsync()
            => await _dbSet.SumAsync(e => e.Amount);

        public async Task<decimal> GetTotalExpensesByDateRangeAsync(DateTime startDate, DateTime endDate)
            => await _dbSet.Where(e => e.Date >= startDate && e.Date <= endDate)
                           .SumAsync(e => e.Amount);
    }
}
