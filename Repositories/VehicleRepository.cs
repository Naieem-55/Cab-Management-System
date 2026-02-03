using Cab_Management_System.Data;
using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Cab_Management_System.Repositories
{
    public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Vehicle>> GetAllAsync()
            => await _dbSet.OrderBy(v => v.RegistrationNumber).ToListAsync();

        public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync()
            => await _dbSet.Where(v => v.Status == VehicleStatus.Available)
                           .OrderBy(v => v.RegistrationNumber)
                           .ToListAsync();

        public async Task<IEnumerable<Vehicle>> SearchVehiclesAsync(string searchTerm)
            => await _dbSet.Where(v => v.RegistrationNumber.Contains(searchTerm) ||
                                       v.Make.Contains(searchTerm) ||
                                       v.Model.Contains(searchTerm) ||
                                       v.Color.Contains(searchTerm))
                           .OrderBy(v => v.RegistrationNumber)
                           .ToListAsync();

        public async Task<Vehicle?> GetVehicleWithMaintenanceAsync(int id)
            => await _dbSet.Include(v => v.MaintenanceRecords)
                           .FirstOrDefaultAsync(v => v.Id == id);
    }
}
