<div align="center">

# Cab Management System

**A comprehensive fleet and operations management platform built with ASP.NET Core MVC**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/EF%20Core-8.0-512BD4?logo=dotnet&logoColor=white)](https://learn.microsoft.com/en-us/ef/core/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?logo=bootstrap&logoColor=white)](https://getbootstrap.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-LocalDB-CC2927?logo=microsoftsqlserver&logoColor=white)](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

---

*Digitize and streamline cab agency operations across fleet management, trip booking, billing, HR, and maintenance -- all under a unified role-based platform.*

</div>

---

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Technology Stack](#technology-stack)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Database Design](#database-design)
- [Modules & Access Control](#modules--access-control)
- [Getting Started](#getting-started)
- [Default Credentials](#default-credentials)
- [Configuration](#configuration)
- [License](#license)

---

## Overview

The Cab Management System is an enterprise-grade web application designed to manage the end-to-end operations of a cab/fleet agency. It provides dedicated workspaces for four operational roles -- **Admin**, **Finance Manager**, **HR Manager**, and **Travel Manager** -- each with tailored dashboards, data views, and workflows.

The platform handles the complete lifecycle of fleet operations: onboarding employees and drivers, managing vehicles and routes, booking and tracking trips, processing payments, and scheduling maintenance.

---

## Key Features

### Core Capabilities
- **Multi-Role Dashboard System** -- Each role gets a dedicated dashboard with KPIs, recent activity feeds, and quick-action links
- **Full CRUD Operations** -- Create, view, edit, and delete across all 7 entity types with form validation
- **Advanced Search & Filtering** -- Text search, enum-based dropdown filters, and date range queries on all list views
- **Pagination** -- Server-side pagination across all list views with configurable page size

### Business Logic
- **Trip Status Workflow** -- Automated driver and vehicle status transitions when trips move between states (Pending, Confirmed, InProgress, Completed, Cancelled)
- **Billing Integration** -- Payment records linked to trips with status tracking (Pending, Completed, Overdue)
- **Maintenance Scheduling** -- Track vehicle service history, upcoming maintenance, and overdue alerts
- **Revenue Reporting** -- Monthly and total revenue calculations with payment method breakdowns

### Technical Highlights
- **Role-Based Authorization** -- ASP.NET Identity with area-level route protection
- **Audit Timestamps** -- Automatic `CreatedDate` / `ModifiedDate` tracking on all entities via EF Core interceptor
- **Structured Logging** -- `ILogger<T>` throughout all service classes with semantic log templates
- **Referential Integrity** -- Configured cascade/restrict delete behaviors to prevent data inconsistency
- **Auto-Migration & Seeding** -- Database schema and sample data provisioned automatically on first run
- **Responsive Design** -- Bootstrap 5 layout with collapsible sidebar, mobile-friendly tables, and status badges

---

## Technology Stack

| Layer | Technology | Purpose |
|-------|-----------|---------|
| **Runtime** | .NET 8.0 | Cross-platform application framework |
| **Web Framework** | ASP.NET Core MVC | Server-side request handling and view rendering |
| **ORM** | Entity Framework Core 8.0 | Object-relational mapping and migrations |
| **Database** | SQL Server (LocalDB) | Relational data storage |
| **Authentication** | ASP.NET Core Identity | User management, password hashing, role claims |
| **Frontend** | Razor Views + Bootstrap 5 | Server-rendered HTML with responsive CSS framework |
| **Icons** | Bootstrap Icons | UI iconography via CDN |
| **Validation** | jQuery Validation Unobtrusive | Client-side form validation tied to server models |

---

## Architecture

The application implements a **layered architecture** with clear separation of concerns:

```
                    ┌─────────────────────────────────────┐
                    │           Presentation               │
                    │   Razor Views  ·  Tag Helpers         │
                    │   Bootstrap 5  ·  Partial Views       │
                    └────────────────┬────────────────────┘
                                     │
                    ┌────────────────▼────────────────────┐
                    │           Controllers                 │
                    │   Area-based  ·  [Authorize(Roles)]   │
                    │   ViewModels  ·  TempData messages    │
                    └────────────────┬────────────────────┘
                                     │
                    ┌────────────────▼────────────────────┐
                    │          Service Layer                │
                    │   Business rules  ·  Orchestration    │
                    │   Status workflows · Logging          │
                    └────────────────┬────────────────────┘
                                     │
                    ┌────────────────▼────────────────────┐
                    │        Repository Layer               │
                    │   Generic CRUD  ·  Eager loading      │
                    │   Custom queries · Filtering          │
                    └────────────────┬────────────────────┘
                                     │
                    ┌────────────────▼────────────────────┐
                    │     EF Core + ASP.NET Identity        │
                    │   DbContext  ·  Migrations             │
                    │   Audit interceptor · Seeding          │
                    └────────────────┬────────────────────┘
                                     │
                    ┌────────────────▼────────────────────┐
                    │           SQL Server                  │
                    └─────────────────────────────────────┘
```

### Design Principles

| Principle | Implementation |
|-----------|---------------|
| **Generic Repository** | `IRepository<T>` / `Repository<T>` provides CRUD, counting, and predicate-based queries; entity-specific repositories extend with custom methods and eager loading |
| **Thin Controllers** | Controllers delegate to services; no direct repository or DbContext access from controllers |
| **Area Isolation** | Each module (Admin, Finance, HR, Travel) lives in its own MVC Area with dedicated controllers, views, and `_ViewImports` |
| **ViewModel Separation** | Domain models never bind directly to forms; ViewModels handle dropdowns, validation display, and form-specific concerns |
| **Automatic Auditing** | `SaveChangesAsync` override in `ApplicationDbContext` sets `CreatedDate` on insert and `ModifiedDate` on update across all entities |

---

## Project Structure

```
Cab Management System/
├── Areas/
│   ├── Admin/
│   │   ├── Controllers/            # Dashboard, UserManagement, Vehicle, Route
│   │   └── Views/                  # 16 views (dashboards, CRUD pages)
│   ├── Finance/
│   │   ├── Controllers/            # Dashboard, Billing, Reports
│   │   └── Views/                  # 7 views
│   ├── HR/
│   │   ├── Controllers/            # Dashboard, Employee, Driver
│   │   └── Views/                  # 11 views
│   └── Travel/
│       ├── Controllers/            # Dashboard, Trip, Maintenance
│       └── Views/                  # 11 views
│
├── Controllers/
│   ├── AccountController.cs        # Authentication (Login, Logout, ChangePassword)
│   └── HomeController.cs           # Landing page with role-based redirect
│
├── Data/
│   ├── ApplicationDbContext.cs     # DbContext, entity config, audit interceptor
│   └── DbSeeder.cs                # Role, user, and sample data seeding
│
├── Models/
│   ├── Enums/                      # 9 enums (DriverStatus, TripStatus, etc.)
│   ├── ViewModels/                 # 12 ViewModels (auth, dashboards, entities)
│   ├── BaseEntity.cs               # Abstract base with audit timestamps
│   ├── PaginatedList.cs            # Generic pagination helper
│   └── [7 domain models]           # Employee, Driver, Vehicle, Route, Trip, Billing, MaintenanceRecord
│
├── Repositories/
│   ├── IRepository.cs              # Generic repository interface
│   ├── Repository.cs               # Generic repository implementation
│   └── [7 entity repositories]     # Interface + implementation pairs
│
├── Services/
│   ├── [8 service interfaces]      # Business operation contracts
│   ├── [7 entity services]         # Business logic implementations
│   └── DashboardService.cs         # Aggregated dashboard statistics
│
├── Views/
│   ├── Account/                    # Login, ChangePassword, AccessDenied
│   ├── Home/                       # Landing page
│   └── Shared/                     # _Layout, _SidebarPartial, _LoginPartial, _PaginationPartial
│
├── wwwroot/                        # Static assets (CSS, JS, client libraries)
├── Migrations/                     # EF Core migration files
├── Program.cs                      # Entry point, DI registration, middleware pipeline
└── appsettings.json                # Connection strings, logging configuration
```

> **Scale:** ~90 C# source files, ~65 Razor views, 13 controllers across 4 areas

---

## Database Design

### Entity Relationship Diagram

```
┌────────────────┐         ┌────────────────┐         ┌────────────────┐
│    Employee     │         │     Driver     │         │      Trip      │
├────────────────┤         ├────────────────┤         ├────────────────┤
│ Id          PK │──1───1──│ Id          PK │──1───*──│ Id          PK │
│ Name           │         │ EmployeeId  FK │         │ DriverId    FK │
│ Email     (UQ) │         │ LicenseNo (UQ) │         │ VehicleId   FK │
│ Phone          │         │ LicenseExpiry  │         │ RouteId     FK │
│ Position       │         │ Status         │         │ CustomerName   │
│ Status         │         │ CreatedDate    │         │ CustomerPhone  │
│ HireDate       │         │ ModifiedDate   │         │ CustomerEmail  │
│ Salary         │         └────────────────┘         │ BookingDate    │
│ CreatedDate    │                                    │ TripDate       │
│ ModifiedDate   │                                    │ Status         │
└────────────────┘                                    │ Cost           │
                                                      │ CreatedDate    │
┌────────────────┐                                    │ ModifiedDate   │
│    Vehicle     │                                    └───────┬────────┘
├────────────────┤                                            │1
│ Id          PK │──1───*── Trip                              │
│ RegNo     (UQ) │                                    ┌───────┴────────┐
│ Make           │                                    │    Billing     │
│ Model          │                                    ├────────────────┤
│ Year           │                                    │ Id          PK │
│ Color          │                                    │ TripId      FK │
│ Capacity       │                                    │ Amount         │
│ FuelType       │                                    │ PaymentDate    │
│ Status         │                                    │ PaymentMethod  │
│ Mileage        │                                    │ Status         │
│ CreatedDate    │                                    │ CreatedDate    │
│ ModifiedDate   │                                    │ ModifiedDate   │
└───────┬────────┘                                    └────────────────┘
        │1
┌───────┴────────┐         ┌────────────────┐
│  Maintenance   │         │     Route      │
│    Record      │         ├────────────────┤
├────────────────┤         │ Id          PK │──1───*── Trip
│ Id          PK │         │ Origin         │
│ VehicleId   FK │         │ Destination    │
│ Description    │         │ Distance       │
│ Cost           │         │ EstTimeHours   │
│ Date           │         │ BaseCost       │
│ NextMaintDate  │         │ CreatedDate    │
│ Status         │         │ ModifiedDate   │
│ CreatedDate    │         └────────────────┘
│ ModifiedDate   │
└────────────────┘

+ ASP.NET Identity tables (AspNetUsers, AspNetRoles, AspNetUserRoles, etc.)
```

### Referential Integrity Rules

| Relationship | Delete Behavior | Rationale |
|-------------|----------------|-----------|
| Employee -> Driver | Restrict | Prevent orphaned driver records |
| Driver -> Trip | Restrict | Prevent deletion of drivers with trip history |
| Vehicle -> Trip | Restrict | Prevent deletion of vehicles with trip history |
| Route -> Trip | Restrict | Prevent deletion of routes with trip history |
| Trip -> Billing | Cascade | Billing is meaningless without its trip |
| Vehicle -> MaintenanceRecord | Cascade | Maintenance history follows vehicle lifecycle |

---

## Modules & Access Control

### Admin Module
> Full system oversight and configuration

| Feature | Description |
|---------|-------------|
| **Dashboard** | System-wide KPIs: total vehicles, drivers, employees, active trips, registered users, total revenue |
| **User Management** | Create and manage system users; assign roles (Admin, FinanceManager, HRManager, TravelManager) |
| **Vehicle Management** | Fleet CRUD with search; track registration, capacity, fuel type, mileage, and availability status |
| **Route Management** | Define origin-destination routes with distance, estimated time, and base cost |

### Finance Module
> Revenue tracking and payment processing

| Feature | Description |
|---------|-------------|
| **Dashboard** | Financial KPIs: total revenue, monthly revenue, pending vs. completed payments |
| **Billing** | Create and manage billing records linked to trips; filter by payment status and date range |
| **Reports** | Revenue summaries with payment method breakdown and period-based analysis |

### HR Module
> Workforce and driver administration

| Feature | Description |
|---------|-------------|
| **Dashboard** | Workforce KPIs: total/active employees, available drivers, employees on leave |
| **Employee Management** | Full CRUD with position (Driver, Receptionist, Mechanic, Cleaner) and status filters |
| **Driver Management** | Register drivers linked to employee records; track license numbers and expiry dates |

### Travel Module
> Trip operations and vehicle maintenance

| Feature | Description |
|---------|-------------|
| **Dashboard** | Operations KPIs: total/active/completed trips, vehicle availability, overdue maintenance count |
| **Trip Management** | Book trips with driver, vehicle, and route selection; automated status workflow updates driver/vehicle availability |
| **Maintenance** | Schedule and track vehicle maintenance; flag overdue service records |

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- SQL Server LocalDB (included with Visual Studio) or any SQL Server instance

### Quick Start

```bash
# 1. Clone the repository
git clone https://github.com/Naieem-55/Cab-Management-System.git
cd "Cab Management System"

# 2. Restore dependencies
dotnet restore

# 3. Run the application
dotnet run
```

The application will automatically:
- Create the database using EF Core migrations
- Seed roles, default users, and sample data

### Access the Application

| Protocol | URL |
|----------|-----|
| HTTPS | `https://localhost:7188` |
| HTTP | `http://localhost:5195` |

### Custom Database Connection

To use a different SQL Server instance, update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=CabManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

---

## Default Credentials

The application seeds the following accounts on first run:

| Role | Email | Password |
|------|-------|----------|
| **Admin** | `admin@cabsystem.com` | `Admin@123` |
| **Finance Manager** | `finance@cabsystem.com` | `Finance@123` |
| **HR Manager** | `hr@cabsystem.com` | `HR@1234` |
| **Travel Manager** | `travel@cabsystem.com` | `Travel@123` |

> **Note:** The Admin account can also create additional users through the User Management module.

### Sample Data

The seeder provisions realistic sample data across all entities:

| Entity | Count | Notes |
|--------|-------|-------|
| Employees | 8 | 4 drivers, 2 receptionists, 1 mechanic, 1 cleaner |
| Drivers | 4 | Linked to driver-position employees |
| Vehicles | 6 | Mixed fuel types and statuses |
| Routes | 5 | Indian city-to-city routes |
| Trips | 6 | Various statuses (Pending through Completed) |
| Billings | 4 | Pending and completed payments |
| Maintenance Records | 3 | Scheduled, in-progress, and completed |

---

## Configuration

### Password Policy

Configured in `Program.cs`:

| Rule | Value |
|------|-------|
| Minimum length | 6 characters |
| Require uppercase | Yes |
| Require lowercase | Yes |
| Require digit | Yes |
| Require special character | Yes |

### Authentication

| Setting | Value |
|---------|-------|
| Login path | `/Account/Login` |
| Access denied path | `/Account/AccessDenied` |
| Session expiry | 30 days (sliding) |

### Logging

Structured logging is enabled across all services using `ILogger<T>`. Log levels are configured in `appsettings.json`:

| Category | Level |
|----------|-------|
| Default | Information |
| Microsoft.AspNetCore | Warning |

---

## License

This project is developed for educational and demonstration purposes.
