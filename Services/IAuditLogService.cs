using Cab_Management_System.Models;

namespace Cab_Management_System.Services
{
    public interface IAuditLogService
    {
        Task<IEnumerable<AuditLog>> GetAllLogsAsync();
        Task<IEnumerable<AuditLog>> GetLogsByEntityAsync(string entityName, int entityId);
        Task<IEnumerable<AuditLog>> SearchLogsAsync(string searchTerm);
    }
}
