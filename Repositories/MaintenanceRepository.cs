using Cab_Management_System.Data;
using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Cab_Management_System.Repositories
{
    public class MaintenanceRepository : Repository<MaintenanceRecord>, IMaintenanceRepository
    {
        public MaintenanceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<MaintenanceRecord>> GetAllAsync()
            => await _dbSet.Include(m => m.Vehicle)
                           .OrderByDescending(m => m.Date)
                           .ToListAsync();

        public async Task<MaintenanceRecord?> GetMaintenanceWithVehicleAsync(int id)
            => await _dbSet.Include(m => m.Vehicle)
                           .FirstOrDefaultAsync(m => m.Id == id);

        public async Task<IEnumerable<MaintenanceRecord>> GetOverdueMaintenanceAsync()
            => await _dbSet.Include(m => m.Vehicle)
                           .Where(m => m.Status == MaintenanceStatus.Overdue ||
                                       (m.NextMaintenanceDate != null && m.NextMaintenanceDate < DateTime.Now &&
                                        m.Status != MaintenanceStatus.Completed))
                           .OrderByDescending(m => m.Date)
                           .ToListAsync();

        public async Task<IEnumerable<MaintenanceRecord>> GetMaintenanceByVehicleAsync(int vehicleId)
            => await _dbSet.Include(m => m.Vehicle)
                           .Where(m => m.VehicleId == vehicleId)
                           .OrderByDescending(m => m.Date)
                           .ToListAsync();
    }
}
