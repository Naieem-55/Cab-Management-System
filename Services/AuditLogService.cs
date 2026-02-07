using Cab_Management_System.Data;
using Cab_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cab_Management_System.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly ApplicationDbContext _context;

        public AuditLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AuditLog>> GetAllLogsAsync()
        {
            return await _context.AuditLogs
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetLogsByEntityAsync(string entityName, int entityId)
        {
            return await _context.AuditLogs
                .Where(a => a.EntityName == entityName && a.EntityId == entityId)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> SearchLogsAsync(string searchTerm)
        {
            var lowerTerm = searchTerm.ToLower();
            return await _context.AuditLogs
                .Where(a => a.EntityName.ToLower().Contains(lowerTerm) ||
                           a.Action.ToLower().Contains(lowerTerm) ||
                           (a.UserName != null && a.UserName.ToLower().Contains(lowerTerm)) ||
                           (a.Changes != null && a.Changes.ToLower().Contains(lowerTerm)))
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }
    }
}
