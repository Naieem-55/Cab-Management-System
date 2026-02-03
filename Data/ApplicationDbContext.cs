using Cab_Management_System.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cab_Management_System.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Models.Route> Routes { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Billing> Billings { get; set; }
        public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }

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
        }
    }
}
