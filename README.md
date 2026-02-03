# Cab Management System

A full-stack web application built with **ASP.NET Core MVC (.NET 8)** to digitize and streamline operations of a travel/car agency. The system supports multiple operational departments with role-based access control, covering vehicle management, driver operations, employee administration, route planning, trip booking, billing, and maintenance tracking.

---

## Table of Contents

- [Features](#features)
- [Technology Stack](#technology-stack)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Database Schema](#database-schema)
- [User Roles & Modules](#user-roles--modules)
- [Getting Started](#getting-started)
- [Default Credentials](#default-credentials)
- [Screenshots](#screenshots)
- [Configuration](#configuration)

---

## Features

- **Role-Based Access Control** -- Four distinct roles (Admin, Finance Manager, HR Manager, Travel Manager) with restricted module access
- **Role-Based Dashboards** -- Each role has a dedicated dashboard with relevant statistics and recent activity
- **Complete CRUD Operations** -- Full Create, Read, Update, Delete functionality for all entities
- **Search & Filter** -- Search bars and dropdown filters on all list pages
- **Responsive UI** -- Bootstrap 5 responsive design with sidebar navigation
- **Authentication & Authorization** -- ASP.NET Identity with secure login, logout, and password management
- **Data Validation** -- Server-side model validation with client-side unobtrusive validation
- **Status Tracking** -- Color-coded status badges across all entities
- **Auto Database Setup** -- Automatic migration and seed data on first run

---

## Technology Stack

| Component | Technology |
|-----------|-----------|
| **Framework** | ASP.NET Core MVC (.NET 8.0) |
| **ORM** | Entity Framework Core 8.0 |
| **Database** | SQL Server (LocalDB for development) |
| **Authentication** | ASP.NET Core Identity |
| **Frontend** | Razor Views, Bootstrap 5, Bootstrap Icons |
| **Validation** | jQuery Validation + Unobtrusive Validation |
| **Architecture** | Repository Pattern + Service Layer |

---

## Architecture

The application follows a layered architecture with clear separation of concerns:

```
┌─────────────────────────────────────────────┐
│                Razor Views                   │
│          (Bootstrap 5 + Tag Helpers)         │
├─────────────────────────────────────────────┤
│              Controllers                     │
│    (Area-based, Role-authorized)             │
├─────────────────────────────────────────────┤
│             Service Layer                    │
│       (Business logic & orchestration)       │
├─────────────────────────────────────────────┤
│           Repository Layer                   │
│     (Data access & query abstraction)        │
├─────────────────────────────────────────────┤
│     Entity Framework Core + Identity         │
│          (ApplicationDbContext)               │
├─────────────────────────────────────────────┤
│             SQL Server                       │
└─────────────────────────────────────────────┘
```

**Key design decisions:**

- **Generic Repository** (`IRepository<T>`) provides common CRUD operations; entity-specific repositories add custom queries with eager loading
- **Service Layer** wraps repository calls and encapsulates business logic, keeping controllers thin
- **MVC Areas** organize modules (Admin, Finance, HR, Travel) into isolated sections with their own controllers and views
- **ViewModels** decouple domain models from view concerns, especially for forms requiring dropdown data

---

## Project Structure

```
Cab Management System/
│
├── Areas/
│   ├── Admin/
│   │   ├── Controllers/          # DashboardController, UserManagementController,
│   │   │                         # VehicleController, RouteController
│   │   └── Views/                # Dashboard, UserManagement, Vehicle, Route views
│   ├── Finance/
│   │   ├── Controllers/          # DashboardController, BillingController, ReportsController
│   │   └── Views/                # Dashboard, Billing, Reports views
│   ├── HR/
│   │   ├── Controllers/          # DashboardController, EmployeeController, DriverController
│   │   └── Views/                # Dashboard, Employee, Driver views
│   └── Travel/
│       ├── Controllers/          # DashboardController, TripController, MaintenanceController
│       └── Views/                # Dashboard, Trip, Maintenance views
│
├── Controllers/
│   ├── AccountController.cs      # Login, Logout, ChangePassword, AccessDenied
│   └── HomeController.cs         # Landing page with role-based redirect
│
├── Data/
│   ├── ApplicationDbContext.cs   # EF Core DbContext with entity configuration
│   └── DbSeeder.cs              # Seeds roles and default admin user
│
├── Models/
│   ├── Enums/                    # DriverStatus, EmployeePosition, EmployeeStatus,
│   │                             # FuelType, MaintenanceStatus, PaymentMethod,
│   │                             # PaymentStatus, TripStatus, VehicleStatus
│   ├── ViewModels/               # Login, Register, ChangePassword, Dashboard VMs,
│   │                             # Trip, Billing, Driver, Maintenance VMs
│   ├── ApplicationUser.cs        # Extends IdentityUser
│   ├── Employee.cs
│   ├── Driver.cs
│   ├── Vehicle.cs
│   ├── Route.cs
│   ├── Trip.cs
│   ├── Billing.cs
│   └── MaintenanceRecord.cs
│
├── Repositories/
│   ├── IRepository.cs            # Generic repository interface
│   ├── Repository.cs             # Generic repository implementation
│   └── [Entity]Repository.cs     # 7 entity-specific repository pairs
│
├── Services/
│   ├── I[Entity]Service.cs       # 8 service interfaces
│   ├── [Entity]Service.cs        # 7 entity service implementations
│   └── DashboardService.cs       # Aggregates stats for role dashboards
│
├── Views/
│   ├── Account/                  # Login, ChangePassword, AccessDenied
│   ├── Home/                     # Landing page, Privacy, Error
│   └── Shared/                   # _Layout, _SidebarPartial, _LoginPartial
│
├── wwwroot/
│   ├── css/site.css              # Custom styles (sidebar, dashboard cards, badges)
│   ├── js/site.js                # Sidebar active link highlighting
│   └── lib/                      # Bootstrap, jQuery, jQuery Validation
│
├── Program.cs                    # Application entry point & DI configuration
├── appsettings.json              # Connection string & logging config
└── Cab Management System.csproj  # Project file with NuGet references
```

**File counts:** 83 C# source files, 65 Razor views

---

## Database Schema

```
┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│   Employee   │     │    Driver    │     │    Trip      │
├──────────────┤     ├──────────────┤     ├──────────────┤
│ Id           │1───1│ Id           │1───*│ Id           │
│ Name         │     │ EmployeeId   │     │ DriverId     │
│ Email (UQ)   │     │ LicenseNo(UQ)│     │ VehicleId    │
│ Phone        │     │ LicenseExpiry│     │ RouteId      │
│ Position     │     │ Status       │     │ CustomerName │
│ Status       │     └──────────────┘     │ CustomerPhone│
│ HireDate     │                          │ BookingDate  │
│ Salary       │     ┌──────────────┐     │ TripDate     │
└──────────────┘     │   Vehicle    │     │ Status       │
                     ├──────────────┤     │ Cost         │
                     │ Id           │1───*│              │
                     │ RegNo (UQ)   │     └──────┬───────┘
                     │ Make         │            │1
                     │ Model        │            │
                     │ Year         │     ┌──────┴───────┐
                     │ Color        │     │   Billing    │
                     │ Capacity     │     ├──────────────┤
                     │ FuelType     │     │ Id           │
                     │ Status       │     │ TripId       │
                     │ Mileage      │     │ Amount       │
                     └──────┬───────┘     │ PaymentDate  │
                            │1            │ PaymentMethod│
                     ┌──────┴───────┐     │ Status       │
                     │ Maintenance  │     └──────────────┘
                     │   Record     │
                     ├──────────────┤     ┌──────────────┐
                     │ Id           │     │    Route     │
                     │ VehicleId    │     ├──────────────┤
                     │ Description  │     │ Id           │1───*  Trip
                     │ Cost         │     │ Origin       │
                     │ Date         │     │ Destination  │
                     │ NextMaintDate│     │ Distance     │
                     │ Status       │     │ EstTimeHours │
                     └──────────────┘     │ BaseCost     │
                                          └──────────────┘

ASP.NET Identity Tables: AspNetUsers, AspNetRoles, AspNetUserRoles,
                          AspNetUserClaims, AspNetRoleClaims,
                          AspNetUserLogins, AspNetUserTokens
```

**Referential integrity:**
- `Restrict` delete on Driver/Vehicle/Route -> Trip (prevents orphaned trips)
- `Restrict` delete on Employee -> Driver
- `Cascade` delete on Trip -> Billing
- `Cascade` delete on Vehicle -> MaintenanceRecord

---

## User Roles & Modules

### Admin
| Feature | Description |
|---------|-------------|
| Dashboard | Overview of all system stats (vehicles, drivers, employees, trips, revenue) |
| User Management | Create, edit, delete manager accounts and assign roles |
| Vehicle Management | Full CRUD for fleet vehicles with status tracking |
| Route Management | Define and manage travel routes with distance and cost |

### Finance Manager
| Feature | Description |
|---------|-------------|
| Dashboard | Revenue stats, pending/completed payment counts, monthly revenue |
| Billing | Create and manage payment records linked to trips |
| Reports | Revenue summaries with totals and payment breakdowns |

### HR Manager
| Feature | Description |
|---------|-------------|
| Dashboard | Employee and driver counts, leave tracking |
| Employee Management | Full CRUD with position and status filters |
| Driver Management | Register drivers linked to employees, track licenses |

### Travel Manager
| Feature | Description |
|---------|-------------|
| Dashboard | Trip stats, vehicle availability, overdue maintenance alerts |
| Trip Management | Book trips by assigning drivers, vehicles, and routes |
| Maintenance | Schedule and track vehicle maintenance records |

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- SQL Server LocalDB (included with Visual Studio) or a SQL Server instance

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd "Cab Management System"
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Update the connection string** (if not using LocalDB)

   Edit `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CabManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

   The database is **created and seeded automatically** on first run via `Database.MigrateAsync()`.

5. **Open in browser**
   - HTTPS: `https://localhost:7188`
   - HTTP: `http://localhost:5195`

### Manual Migration (if needed)

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## Default Credentials

| Role | Email | Password |
|------|-------|----------|
| **Admin** | admin@cabsystem.com | Admin@123 |

After logging in as Admin, use **User Management** to create accounts for Finance Manager, HR Manager, and Travel Manager roles.

---

## Screenshots

After running the application, you can explore:

- **Landing Page** -- Public homepage with feature overview and login button
- **Admin Dashboard** -- Stats cards showing vehicles, drivers, employees, trips, and revenue
- **Vehicle Management** -- Searchable table with status badges and full CRUD
- **Trip Booking** -- Form with dropdown selectors for driver, vehicle, and route
- **Billing Management** -- Payment tracking with status filters and revenue reports
- **Employee Management** -- Filterable employee list with position and status dropdowns

---

## Configuration

### Password Policy

Configured in `Program.cs`:
- Minimum 6 characters
- Requires uppercase, lowercase, digit, and special character

### Cookie Settings

- Login path: `/Account/Login`
- Access denied path: `/Account/AccessDenied`

### Logging

Default logging levels in `appsettings.json`:
- General: `Information`
- ASP.NET Core: `Warning`

---

## License

This project is developed for educational and demonstration purposes.
