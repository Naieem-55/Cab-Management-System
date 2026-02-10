using Cab_Management_System.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace Cab_Management_System.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor? httpContextAccessor = null)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Models.Route> Routes { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Billing> Billings { get; set; }
        public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<DriverRating> DriverRatings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Driver>()
                .HasOne(d => d.Employee)
                .WithOne(e => e.Driver)
                .HasForeignKey<Driver>(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Trip>()
                .HasOne(t => t.Driver)
                .WithMany(d => d.Trips)
                .HasForeignKey(t => t.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Trip>()
                .HasOne(t => t.Vehicle)
                .WithMany(v => v.Trips)
                .HasForeignKey(t => t.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Trip>()
                .HasOne(t => t.Route)
                .WithMany(r => r.Trips)
                .HasForeignKey(t => t.RouteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Billing>()
                .HasOne(b => b.Trip)
                .WithOne(t => t.Billing)
                .HasForeignKey<Billing>(b => b.TripId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<MaintenanceRecord>()
                .HasOne(m => m.Vehicle)
                .WithMany(v => v.MaintenanceRecords)
                .HasForeignKey(m => m.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Vehicle>()
                .HasIndex(v => v.RegistrationNumber)
                .IsUnique();

            builder.Entity<Driver>()
                .HasIndex(d => d.LicenseNumber)
                .IsUnique();

            builder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            builder.Entity<Employee>()
                .Property(e => e.Salary)
                .HasPrecision(18, 2);

            builder.Entity<Models.Route>()
                .Property(r => r.BaseCost)
                .HasPrecision(18, 2);

            builder.Entity<Trip>()
                .Property(t => t.Cost)
                .HasPrecision(18, 2);

            builder.Entity<Billing>()
                .Property(b => b.Amount)
                .HasPrecision(18, 2);

            builder.Entity<MaintenanceRecord>()
                .Property(m => m.Cost)
                .HasPrecision(18, 2);

            // Customer configuration
            builder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

            builder.Entity<Trip>()
                .HasOne(t => t.Customer)
                .WithMany(c => c.Trips)
                .HasForeignKey(t => t.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);

            // Expense configuration
            builder.Entity<Expense>()
                .HasOne(e => e.Vehicle)
                .WithMany(v => v.Expenses)
                .HasForeignKey(e => e.VehicleId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Expense>()
                .HasOne(e => e.Trip)
                .WithMany(t => t.Expenses)
                .HasForeignKey(e => e.TripId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasPrecision(18, 2);

            // DriverRating configuration
            builder.Entity<DriverRating>()
                .HasOne(dr => dr.Trip)
                .WithOne(t => t.DriverRating)
                .HasForeignKey<DriverRating>(dr => dr.TripId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<DriverRating>()
                .HasOne(dr => dr.Driver)
                .WithMany(d => d.Ratings)
                .HasForeignKey(dr => dr.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DriverRating>()
                .HasIndex(dr => dr.TripId)
                .IsUnique();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var auditEntries = new List<AuditLog>();
            var userId = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;

            var entries = ChangeTracker.Entries<BaseEntity>();
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.Now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedDate = DateTime.Now;
                    entry.Property(nameof(BaseEntity.CreatedDate)).IsModified = false;
                }
            }

            // Capture audit entries before save
            foreach (var entry in ChangeTracker.Entries<BaseEntity>().ToList())
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                {
                    var entityName = entry.Entity.GetType().Name;
                    var action = entry.State switch
                    {
                        EntityState.Added => "Create",
                        EntityState.Modified => "Update",
                        EntityState.Deleted => "Delete",
                        _ => "Unknown"
                    };

                    var changes = new Dictionary<string, object?>();

                    if (entry.State == EntityState.Modified)
                    {
                        foreach (var prop in entry.Properties.Where(p => p.IsModified && p.Metadata.Name != "ModifiedDate"))
                        {
                            changes[prop.Metadata.Name] = new { Old = prop.OriginalValue, New = prop.CurrentValue };
                        }
                    }
                    else if (entry.State == EntityState.Added)
                    {
                        foreach (var prop in entry.Properties.Where(p => p.CurrentValue != null && p.Metadata.Name != "CreatedDate" && p.Metadata.Name != "ModifiedDate"))
                        {
                            changes[prop.Metadata.Name] = prop.CurrentValue;
                        }
                    }
                    else if (entry.State == EntityState.Deleted)
                    {
                        foreach (var prop in entry.Properties.Where(p => p.OriginalValue != null))
                        {
                            changes[prop.Metadata.Name] = prop.OriginalValue;
                        }
                    }

                    auditEntries.Add(new AuditLog
                    {
                        EntityName = entityName,
                        EntityId = entry.State == EntityState.Added ? 0 : (int)(entry.Property("Id").CurrentValue ?? 0),
                        Action = action,
                        Changes = changes.Count > 0 ? JsonSerializer.Serialize(changes) : null,
                        UserId = userId,
                        UserName = userName,
                        Timestamp = DateTime.Now
                    });
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);

            // Save audit logs after main save (so new entity IDs are available)
            if (auditEntries.Count > 0)
            {
                // For newly added entities, we stored a temp reference - get IDs from tracker
                var addedEntities = ChangeTracker.Entries<BaseEntity>()
                    .Where(e => e.State == EntityState.Unchanged)
                    .ToList();

                foreach (var audit in auditEntries.Where(a => a.Action == "Create" && a.EntityId == 0))
                {
                    var match = addedEntities.FirstOrDefault(e => e.Entity.GetType().Name == audit.EntityName);
                    if (match != null)
                    {
                        audit.EntityId = (int)(match.Property("Id").CurrentValue ?? 0);
                    }
                }

                AuditLogs.AddRange(auditEntries);
                await base.SaveChangesAsync(cancellationToken);
            }

            return result;
        }
    }
}
