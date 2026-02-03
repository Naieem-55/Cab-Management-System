using Cab_Management_System.Data;
using Cab_Management_System.Models;
using Cab_Management_System.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cab_Management_System.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = { "Admin", "FinanceManager", "HRManager", "TravelManager" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "admin@cabsystem.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Administrator",
                    Role = "Admin",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Seed manager users and sample data
            await SeedSampleDataAsync(serviceProvider);
        }

        private static async Task SeedSampleDataAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Check if data already exists - skip seeding if Employees table has any rows
            if (await context.Employees.AnyAsync())
            {
                return;
            }

            // ---------------------------------------------------------------
            // 1. Seed Manager Users
            // ---------------------------------------------------------------
            var managers = new[]
            {
                new { Email = "finance@cabsystem.com", Password = "Finance@123", FirstName = "Anita", LastName = "Verma", Role = "FinanceManager" },
                new { Email = "hr@cabsystem.com", Password = "HR@1234", FirstName = "Vikram", LastName = "Mehta", Role = "HRManager" },
                new { Email = "travel@cabsystem.com", Password = "Travel@123", FirstName = "Deepa", LastName = "Nair", Role = "TravelManager" }
            };

            foreach (var mgr in managers)
            {
                var existingUser = await userManager.FindByEmailAsync(mgr.Email);
                if (existingUser == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = mgr.Email,
                        Email = mgr.Email,
                        FirstName = mgr.FirstName,
                        LastName = mgr.LastName,
                        Role = mgr.Role,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, mgr.Password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, mgr.Role);
                    }
                }
            }

            // ---------------------------------------------------------------
            // 2. Seed Employees (8 total)
            // ---------------------------------------------------------------
            var employees = new List<Employee>
            {
                // 4 Drivers
                new Employee
                {
                    Name = "Rajesh Kumar",
                    Email = "rajesh.kumar@cabsystem.com",
                    Phone = "9876543210",
                    Position = EmployeePosition.Driver,
                    Status = EmployeeStatus.Active,
                    HireDate = new DateTime(2021, 3, 15),
                    Salary = 25000m
                },
                new Employee
                {
                    Name = "Amit Singh",
                    Email = "amit.singh@cabsystem.com",
                    Phone = "9876543211",
                    Position = EmployeePosition.Driver,
                    Status = EmployeeStatus.Active,
                    HireDate = new DateTime(2020, 7, 1),
                    Salary = 28000m
                },
                new Employee
                {
                    Name = "Priya Sharma",
                    Email = "priya.sharma@cabsystem.com",
                    Phone = "9876543212",
                    Position = EmployeePosition.Driver,
                    Status = EmployeeStatus.Active,
                    HireDate = new DateTime(2022, 1, 10),
                    Salary = 24000m
                },
                new Employee
                {
                    Name = "Suresh Patel",
                    Email = "suresh.patel@cabsystem.com",
                    Phone = "9876543213",
                    Position = EmployeePosition.Driver,
                    Status = EmployeeStatus.Active,
                    HireDate = new DateTime(2019, 11, 20),
                    Salary = 30000m
                },
                // 2 Receptionists
                new Employee
                {
                    Name = "Neha Gupta",
                    Email = "neha.gupta@cabsystem.com",
                    Phone = "9876543214",
                    Position = EmployeePosition.Receptionist,
                    Status = EmployeeStatus.Active,
                    HireDate = new DateTime(2023, 2, 1),
                    Salary = 20000m
                },
                new Employee
                {
                    Name = "Kavita Rao",
                    Email = "kavita.rao@cabsystem.com",
                    Phone = "9876543215",
                    Position = EmployeePosition.Receptionist,
                    Status = EmployeeStatus.Active,
                    HireDate = new DateTime(2022, 6, 15),
                    Salary = 21000m
                },
                // 1 Mechanic
                new Employee
                {
                    Name = "Ramesh Yadav",
                    Email = "ramesh.yadav@cabsystem.com",
                    Phone = "9876543216",
                    Position = EmployeePosition.Mechanic,
                    Status = EmployeeStatus.Active,
                    HireDate = new DateTime(2020, 4, 10),
                    Salary = 27000m
                },
                // 1 Cleaner
                new Employee
                {
                    Name = "Sunita Devi",
                    Email = "sunita.devi@cabsystem.com",
                    Phone = "9876543217",
                    Position = EmployeePosition.Cleaner,
                    Status = EmployeeStatus.Active,
                    HireDate = new DateTime(2023, 8, 5),
                    Salary = 15000m
                }
            };

            context.Employees.AddRange(employees);
            await context.SaveChangesAsync();

            // ---------------------------------------------------------------
            // 3. Seed Drivers (4, linked to Driver-position employees)
            // ---------------------------------------------------------------
            var driverEmployees = await context.Employees
                .Where(e => e.Position == EmployeePosition.Driver)
                .OrderBy(e => e.Id)
                .ToListAsync();

            var drivers = new List<Driver>
            {
                new Driver
                {
                    EmployeeId = driverEmployees[0].Id,
                    LicenseNumber = "DL-1420-2019-0012345",
                    LicenseExpiry = new DateTime(2027, 3, 14),
                    Status = DriverStatus.Available
                },
                new Driver
                {
                    EmployeeId = driverEmployees[1].Id,
                    LicenseNumber = "DL-0520-2018-0067890",
                    LicenseExpiry = new DateTime(2027, 6, 30),
                    Status = DriverStatus.Available
                },
                new Driver
                {
                    EmployeeId = driverEmployees[2].Id,
                    LicenseNumber = "DL-0922-2021-0034567",
                    LicenseExpiry = new DateTime(2028, 1, 9),
                    Status = DriverStatus.Available
                },
                new Driver
                {
                    EmployeeId = driverEmployees[3].Id,
                    LicenseNumber = "DL-1119-2017-0098765",
                    LicenseExpiry = new DateTime(2026, 11, 19),
                    Status = DriverStatus.OnTrip
                }
            };

            context.Drivers.AddRange(drivers);
            await context.SaveChangesAsync();

            // ---------------------------------------------------------------
            // 4. Seed Vehicles (6 total)
            // ---------------------------------------------------------------
            var vehicles = new List<Vehicle>
            {
                new Vehicle
                {
                    RegistrationNumber = "MH-01-AB-1234",
                    Make = "Toyota",
                    Model = "Innova",
                    Year = 2022,
                    Color = "White",
                    Capacity = 7,
                    FuelType = FuelType.Diesel,
                    Status = VehicleStatus.Available,
                    Mileage = 45000.0
                },
                new Vehicle
                {
                    RegistrationNumber = "MH-02-CD-5678",
                    Make = "Honda",
                    Model = "City",
                    Year = 2023,
                    Color = "Silver",
                    Capacity = 4,
                    FuelType = FuelType.Petrol,
                    Status = VehicleStatus.Available,
                    Mileage = 22000.0
                },
                new Vehicle
                {
                    RegistrationNumber = "DL-03-EF-9012",
                    Make = "Hyundai",
                    Model = "Creta",
                    Year = 2021,
                    Color = "Blue",
                    Capacity = 5,
                    FuelType = FuelType.Diesel,
                    Status = VehicleStatus.Available,
                    Mileage = 58000.0
                },
                new Vehicle
                {
                    RegistrationNumber = "KA-04-GH-3456",
                    Make = "Maruti",
                    Model = "Swift",
                    Year = 2024,
                    Color = "Red",
                    Capacity = 4,
                    FuelType = FuelType.Petrol,
                    Status = VehicleStatus.Available,
                    Mileage = 8000.0
                },
                new Vehicle
                {
                    RegistrationNumber = "TN-05-IJ-7890",
                    Make = "Tata",
                    Model = "Nexon",
                    Year = 2023,
                    Color = "Black",
                    Capacity = 5,
                    FuelType = FuelType.CNG,
                    Status = VehicleStatus.OnTrip,
                    Mileage = 35000.0
                },
                new Vehicle
                {
                    RegistrationNumber = "AP-06-KL-2345",
                    Make = "Maruti",
                    Model = "Ertiga",
                    Year = 2020,
                    Color = "Grey",
                    Capacity = 7,
                    FuelType = FuelType.CNG,
                    Status = VehicleStatus.InMaintenance,
                    Mileage = 72000.0
                }
            };

            context.Vehicles.AddRange(vehicles);
            await context.SaveChangesAsync();

            // ---------------------------------------------------------------
            // 5. Seed Routes (5 total)
            // ---------------------------------------------------------------
            var routes = new List<Models.Route>
            {
                new Models.Route
                {
                    Origin = "Mumbai",
                    Destination = "Pune",
                    Distance = 150.0,
                    EstimatedTimeHours = 3.0,
                    BaseCost = 2500m
                },
                new Models.Route
                {
                    Origin = "Delhi",
                    Destination = "Agra",
                    Distance = 230.0,
                    EstimatedTimeHours = 4.0,
                    BaseCost = 3500m
                },
                new Models.Route
                {
                    Origin = "Bangalore",
                    Destination = "Mysore",
                    Distance = 150.0,
                    EstimatedTimeHours = 3.0,
                    BaseCost = 2200m
                },
                new Models.Route
                {
                    Origin = "Chennai",
                    Destination = "Pondicherry",
                    Distance = 170.0,
                    EstimatedTimeHours = 3.5,
                    BaseCost = 2800m
                },
                new Models.Route
                {
                    Origin = "Hyderabad",
                    Destination = "Warangal",
                    Distance = 150.0,
                    EstimatedTimeHours = 3.0,
                    BaseCost = 2300m
                }
            };

            context.Routes.AddRange(routes);
            await context.SaveChangesAsync();

            // Reload drivers, vehicles, and routes to get their generated IDs
            var savedDrivers = await context.Drivers.OrderBy(d => d.Id).ToListAsync();
            var savedVehicles = await context.Vehicles.OrderBy(v => v.Id).ToListAsync();
            var savedRoutes = await context.Routes.OrderBy(r => r.Id).ToListAsync();

            // ---------------------------------------------------------------
            // 6. Seed Trips (6 total)
            // ---------------------------------------------------------------
            var trips = new List<Trip>
            {
                // Trip 1 - Completed (Mumbai-Pune)
                new Trip
                {
                    DriverId = savedDrivers[0].Id,
                    VehicleId = savedVehicles[0].Id,
                    RouteId = savedRoutes[0].Id,
                    CustomerName = "Anil Kapoor",
                    CustomerPhone = "9812345001",
                    CustomerEmail = "anil.kapoor@email.com",
                    BookingDate = new DateTime(2025, 12, 1),
                    TripDate = new DateTime(2025, 12, 5),
                    Status = TripStatus.Completed,
                    Cost = 2500m
                },
                // Trip 2 - Completed (Delhi-Agra)
                new Trip
                {
                    DriverId = savedDrivers[1].Id,
                    VehicleId = savedVehicles[1].Id,
                    RouteId = savedRoutes[1].Id,
                    CustomerName = "Meera Joshi",
                    CustomerPhone = "9812345002",
                    CustomerEmail = "meera.joshi@email.com",
                    BookingDate = new DateTime(2025, 12, 10),
                    TripDate = new DateTime(2025, 12, 15),
                    Status = TripStatus.Completed,
                    Cost = 3600m
                },
                // Trip 3 - InProgress (Bangalore-Mysore)
                new Trip
                {
                    DriverId = savedDrivers[2].Id,
                    VehicleId = savedVehicles[2].Id,
                    RouteId = savedRoutes[2].Id,
                    CustomerName = "Ravi Shankar",
                    CustomerPhone = "9812345003",
                    CustomerEmail = "ravi.shankar@email.com",
                    BookingDate = new DateTime(2026, 1, 20),
                    TripDate = new DateTime(2026, 1, 28),
                    Status = TripStatus.InProgress,
                    Cost = 2200m
                },
                // Trip 4 - InProgress (Chennai-Pondicherry)
                new Trip
                {
                    DriverId = savedDrivers[3].Id,
                    VehicleId = savedVehicles[4].Id,
                    RouteId = savedRoutes[3].Id,
                    CustomerName = "Lakshmi Prasad",
                    CustomerPhone = "9812345004",
                    CustomerEmail = "lakshmi.prasad@email.com",
                    BookingDate = new DateTime(2026, 1, 25),
                    TripDate = new DateTime(2026, 2, 1),
                    Status = TripStatus.InProgress,
                    Cost = 2900m
                },
                // Trip 5 - Pending (Hyderabad-Warangal)
                new Trip
                {
                    DriverId = savedDrivers[0].Id,
                    VehicleId = savedVehicles[3].Id,
                    RouteId = savedRoutes[4].Id,
                    CustomerName = "Sanjay Dutt",
                    CustomerPhone = "9812345005",
                    CustomerEmail = "sanjay.dutt@email.com",
                    BookingDate = new DateTime(2026, 2, 1),
                    TripDate = new DateTime(2026, 2, 10),
                    Status = TripStatus.Pending,
                    Cost = 2300m
                },
                // Trip 6 - Confirmed (Mumbai-Pune)
                new Trip
                {
                    DriverId = savedDrivers[1].Id,
                    VehicleId = savedVehicles[0].Id,
                    RouteId = savedRoutes[0].Id,
                    CustomerName = "Fatima Sheikh",
                    CustomerPhone = "9812345006",
                    CustomerEmail = "fatima.sheikh@email.com",
                    BookingDate = new DateTime(2026, 2, 2),
                    TripDate = new DateTime(2026, 2, 8),
                    Status = TripStatus.Confirmed,
                    Cost = 2600m
                }
            };

            context.Trips.AddRange(trips);
            await context.SaveChangesAsync();

            // Reload trips to get their generated IDs
            var savedTrips = await context.Trips.OrderBy(t => t.Id).ToListAsync();

            // ---------------------------------------------------------------
            // 7. Seed Billings (4 total - for completed and in-progress trips)
            // ---------------------------------------------------------------
            var billings = new List<Billing>
            {
                // Billing for Trip 1 (Completed) - Payment Completed, Cash
                new Billing
                {
                    TripId = savedTrips[0].Id,
                    Amount = 2500m,
                    PaymentDate = new DateTime(2025, 12, 5),
                    PaymentMethod = PaymentMethod.Cash,
                    Status = PaymentStatus.Completed
                },
                // Billing for Trip 2 (Completed) - Payment Completed, CreditCard
                new Billing
                {
                    TripId = savedTrips[1].Id,
                    Amount = 3600m,
                    PaymentDate = new DateTime(2025, 12, 15),
                    PaymentMethod = PaymentMethod.CreditCard,
                    Status = PaymentStatus.Completed
                },
                // Billing for Trip 3 (InProgress) - Payment Pending, BankTransfer
                new Billing
                {
                    TripId = savedTrips[2].Id,
                    Amount = 2200m,
                    PaymentDate = new DateTime(2026, 1, 28),
                    PaymentMethod = PaymentMethod.BankTransfer,
                    Status = PaymentStatus.Pending
                },
                // Billing for Trip 4 (InProgress) - Payment Pending, Cash
                new Billing
                {
                    TripId = savedTrips[3].Id,
                    Amount = 2900m,
                    PaymentDate = new DateTime(2026, 2, 1),
                    PaymentMethod = PaymentMethod.Cash,
                    Status = PaymentStatus.Pending
                }
            };

            context.Billings.AddRange(billings);
            await context.SaveChangesAsync();

            // ---------------------------------------------------------------
            // 8. Seed MaintenanceRecords (3 total)
            // ---------------------------------------------------------------
            var maintenanceRecords = new List<MaintenanceRecord>
            {
                // Completed maintenance (past date) - for the InMaintenance vehicle (Ertiga)
                new MaintenanceRecord
                {
                    VehicleId = savedVehicles[5].Id,
                    Description = "Oil change and filter replacement",
                    Cost = 3500m,
                    Date = new DateTime(2025, 11, 15),
                    NextMaintenanceDate = new DateTime(2026, 5, 15),
                    Status = MaintenanceStatus.Completed
                },
                // Scheduled maintenance (future date)
                new MaintenanceRecord
                {
                    VehicleId = savedVehicles[0].Id,
                    Description = "Brake pad replacement",
                    Cost = 5000m,
                    Date = new DateTime(2026, 3, 10),
                    NextMaintenanceDate = new DateTime(2026, 9, 10),
                    Status = MaintenanceStatus.Scheduled
                },
                // InProgress maintenance (current) - for the InMaintenance vehicle (Ertiga)
                new MaintenanceRecord
                {
                    VehicleId = savedVehicles[5].Id,
                    Description = "Full service and inspection",
                    Cost = 8000m,
                    Date = new DateTime(2026, 2, 1),
                    NextMaintenanceDate = new DateTime(2026, 8, 1),
                    Status = MaintenanceStatus.InProgress
                }
            };

            context.MaintenanceRecords.AddRange(maintenanceRecords);
            await context.SaveChangesAsync();
        }
    }
}
