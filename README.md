<div align="center">

# Cab Management System

**Enterprise-grade fleet and operations management platform built with ASP.NET Core 8 MVC**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/EF%20Core-8.0-512BD4?logo=dotnet&logoColor=white)](https://learn.microsoft.com/en-us/ef/core/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?logo=bootstrap&logoColor=white)](https://getbootstrap.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-LocalDB-CC2927?logo=microsoftsqlserver&logoColor=white)](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
[![MailKit](https://img.shields.io/badge/MailKit-4.3-blue)](https://github.com/jstedfast/MailKit)
[![QuestPDF](https://img.shields.io/badge/QuestPDF-2024.3-orange)](https://www.questpdf.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

---

*A unified, role-based platform for managing fleet operations, trip booking, customer relationships, billing, expense tracking, HR, and vehicle maintenance.*

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

The Cab Management System is a full-featured web application designed to digitize and streamline the end-to-end operations of a cab or fleet agency. It provides dedicated workspaces for four operational roles -- **Admin**, **Finance Manager**, **HR Manager**, and **Travel Manager** -- each with tailored dashboards, analytics, and workflows.

The platform covers the complete operational lifecycle: managing customers, onboarding employees and drivers, maintaining vehicles and routes, booking and tracking trips, processing payments, tracking expenses, generating PDF invoices, monitoring license compliance, and delivering email notifications.

---

## Key Features

### Core Operations
- **Multi-Role Dashboard System** -- Four dedicated dashboards with role-specific KPIs, interactive Chart.js visualizations, activity feeds, and contextual alerts
- **Full CRUD Operations** -- Create, view, edit, and delete across all 9 entity types with server-side validation and error handling
- **Advanced Search & Filtering** -- Text search, enum-based dropdown filters, date range queries, and multi-criteria filtering on all list views
- **Server-Side Pagination** -- Consistent pagination across all list views with query string preservation

### Customer & Trip Management
- **Customer Profiles** -- Centralized customer directory with contact details, trip history, and total spend tracking
- **Trip Booking Workflow** -- Automated driver and vehicle status transitions as trips progress through states (Pending, Confirmed, InProgress, Completed, Cancelled)
- **Customer-Trip Linking** -- Optional association between customer profiles and trip bookings for repeat customer tracking

### Financial Management
- **Billing & Payments** -- Payment records linked to trips with status tracking, payment method breakdown, and date-range filtering
- **Expense Tracking** -- Categorized expense recording (Fuel, Toll, Parking, Driver Allowance, Insurance, Other) with optional vehicle and trip associations
- **Profit & Loss Analysis** -- Real-time net profit calculations on the Finance dashboard and Reports page (revenue minus expenses)
- **Revenue Reporting** -- Monthly and total revenue summaries with payment method distribution and trend analysis
- **PDF Invoice Generation** -- Professional A4 invoices generated via QuestPDF with company branding, trip details, and billing summary

### HR & Compliance
- **Employee Management** -- Full workforce management with position classification and employment status tracking
- **Driver Management** -- Driver registration linked to employee records with license number and expiry date tracking
- **License Expiry Alerts** -- Proactive alerts on Admin and HR dashboards for drivers with licenses expiring within 30 days or already expired
- **Visual Compliance Badges** -- "Expired" and "Expiring Soon" badges displayed directly on the Driver Index for at-a-glance compliance monitoring

### Communication
- **Email Notifications** -- MailKit-powered transactional email delivery with graceful degradation when SMTP is not configured
- **Password Reset Emails** -- Secure password reset flow with tokenized email links replacing raw URL display
- **Booking Confirmations** -- Automated email confirmations sent to customers upon trip creation

### Platform Capabilities
- **Role-Based Authorization** -- ASP.NET Identity with area-level route protection and per-controller role enforcement
- **Comprehensive Audit Trail** -- Automatic change tracking via `SaveChangesAsync` override, recording entity name, action type, changed fields, and user identity
- **CSV Data Export** -- One-click export to CSV on Customers, Vehicles, Trips, Billings, and Expenses
- **Security Headers** -- X-Frame-Options, X-Content-Type-Options, X-XSS-Protection, Referrer-Policy, and Content-Security-Policy middleware
- **Structured Logging** -- `ILogger<T>` throughout all controllers and services with semantic log templates
- **Auto-Migration & Seeding** -- Database schema and sample data provisioned automatically on first run
- **Responsive Design** -- Bootstrap 5 layout with collapsible sidebar, mobile-friendly tables, and status badges

---

## Technology Stack

| Layer | Technology | Purpose |
|-------|-----------|---------|
| **Runtime** | .NET 8.0 | Cross-platform application framework |
| **Web Framework** | ASP.NET Core MVC | Server-side request handling and view rendering |
| **ORM** | Entity Framework Core 8.0 | Object-relational mapping, migrations, and change tracking |
| **Database** | SQL Server (LocalDB) | Relational data storage |
| **Authentication** | ASP.NET Core Identity | User management, password hashing, role-based claims |
| **Email** | MailKit 4.3 | SMTP email delivery for notifications and password resets |
| **PDF Generation** | QuestPDF 2024.3 | Fluent API for generating professional PDF invoices |
| **Frontend** | Razor Views + Bootstrap 5 | Server-rendered HTML with responsive CSS framework |
| **Charts** | Chart.js 4.4 | Interactive dashboard visualizations (pie, bar, line) |
| **Icons** | Bootstrap Icons | UI iconography via CDN |
| **Validation** | jQuery Validation Unobtrusive | Client-side form validation tied to server data annotations |

---

## Architecture

The application implements a **layered architecture** with clear separation of concerns:

```
                    +-------------------------------------+
                    |           Presentation               |
                    |   Razor Views  .  Tag Helpers         |
                    |   Bootstrap 5  .  Chart.js            |
                    +----------------+--------------------+
                                     |
                    +----------------v--------------------+
                    |           Controllers                 |
                    |   Area-based  .  [Authorize(Roles)]   |
                    |   ViewModels  .  TempData messages    |
                    +----------------+--------------------+
                                     |
                    +----------------v--------------------+
                    |          Service Layer                |
                    |   Business rules  .  Orchestration    |
                    |   Email  .  PDF  .  Logging           |
                    +----------------+--------------------+
                                     |
                    +----------------v--------------------+
                    |        Repository Layer               |
                    |   Generic CRUD  .  Eager loading      |
                    |   Custom queries . Filtering          |
                    +----------------+--------------------+
                                     |
                    +----------------v--------------------+
                    |     EF Core + ASP.NET Identity        |
                    |   DbContext  .  Migrations             |
                    |   Audit interceptor . Seeding          |
                    +----------------+--------------------+
                                     |
                    +----------------v--------------------+
                    |           SQL Server                  |
                    +-------------------------------------+
```

### Design Principles

| Principle | Implementation |
|-----------|---------------|
| **Generic Repository** | `IRepository<T>` / `Repository<T>` provides CRUD, counting, and predicate-based queries; entity-specific repositories extend with custom methods and eager loading |
| **Thin Controllers** | Controllers delegate to services; no direct repository or DbContext access from controllers |
| **Area Isolation** | Each module (Admin, Finance, HR, Travel) resides in its own MVC Area with dedicated controllers, views, and `_ViewImports` |
| **ViewModel Separation** | Domain models never bind directly to forms; ViewModels handle dropdowns, validation display, and form-specific concerns |
| **Automatic Auditing** | `SaveChangesAsync` override in `ApplicationDbContext` sets `CreatedDate` on insert and `ModifiedDate` on update, with full change logging to the AuditLog table |
| **Graceful Degradation** | Email and external services are wrapped in try-catch blocks to ensure core operations are never disrupted by third-party failures |

---

## Project Structure

```
Cab Management System/
|-- Areas/
|   |-- Admin/
|   |   |-- Controllers/            # Dashboard, UserManagement, Vehicle, Route, Customer
|   |   +-- Views/                  # 21 views (dashboards, CRUD, customer management)
|   |-- Finance/
|   |   |-- Controllers/            # Dashboard, Billing, Expense, Reports
|   |   +-- Views/                  # 17 views (dashboards, CRUD, expense tracking)
|   |-- HR/
|   |   |-- Controllers/            # Dashboard, Employee, Driver
|   |   +-- Views/                  # 11 views (dashboards, CRUD, license alerts)
|   +-- Travel/
|       |-- Controllers/            # Dashboard, Trip, Maintenance
|       +-- Views/                  # 11 views (dashboards, CRUD, booking)
|
|-- Controllers/
|   |-- AccountController.cs        # Authentication, profile, password reset with email
|   +-- HomeController.cs           # Landing page with role-based redirect
|
|-- Data/
|   |-- ApplicationDbContext.cs     # DbContext, entity config, audit interceptor
|   +-- DbSeeder.cs                # Role, user, and sample data seeding
|
|-- Helpers/
|   +-- CsvExportHelper.cs         # Generic CSV export utility
|
|-- Models/
|   |-- Enums/                      # 10 enums (DriverStatus, TripStatus, ExpenseCategory, etc.)
|   |-- ViewModels/                 # 14 ViewModels (auth, dashboards, entities)
|   |-- BaseEntity.cs               # Abstract base with audit timestamps
|   |-- EmailSettings.cs            # SMTP configuration POCO
|   |-- PaginatedList.cs            # Generic pagination helper
|   +-- [9 domain models]           # Employee, Driver, Vehicle, Route, Trip, Billing,
|                                   # MaintenanceRecord, Customer, Expense
|
|-- Repositories/
|   |-- IRepository.cs              # Generic repository interface
|   |-- Repository.cs               # Generic repository implementation
|   +-- [9 entity repositories]     # Interface + implementation pairs
|
|-- Services/
|   |-- IEmailService.cs            # Email notification contract
|   |-- EmailService.cs             # MailKit SMTP implementation
|   |-- IInvoicePdfService.cs       # PDF generation contract
|   |-- InvoicePdfService.cs        # QuestPDF invoice implementation
|   |-- IDashboardService.cs        # Dashboard aggregation contract
|   |-- DashboardService.cs         # Multi-role dashboard statistics
|   +-- [9 entity services]         # Business logic implementations
|
|-- Views/
|   |-- Account/                    # Login, ChangePassword, ForgotPassword, ResetPassword, Profile
|   |-- Home/                       # Landing page
|   +-- Shared/                     # _Layout, _SidebarPartial, _LoginPartial, _PaginationPartial
|
|-- wwwroot/                        # Static assets (CSS, JS, client libraries)
|-- Migrations/                     # EF Core migration files
|-- Program.cs                      # Entry point, DI registration, middleware pipeline
+-- appsettings.json                # Connection strings, logging, email settings
```

---

## Database Design

### Entity Relationship Diagram

```
+----------------+         +----------------+         +----------------+
|    Employee    |         |     Driver     |         |    Customer    |
+----------------+         +----------------+         +----------------+
| Id          PK |--1---1--| Id          PK |         | Id          PK |
| Name           |         | EmployeeId  FK |         | Name           |
| Email     (UQ) |         | LicenseNo (UQ) |         | Email     (UQ) |
| Phone          |         | LicenseExpiry  |         | Phone          |
| Position       |         | Status         |         | Address        |
| Status         |         +----------------+         +-------+--------+
| HireDate       |                |                           |
| Salary         |                1                           |
+----------------+                |                           |
                                  *                       0..* (optional)
+----------------+         +------+----------+                |
|    Vehicle     |         |      Trip       |<-------+-------+
+----------------+         +-----------------+        |
| Id          PK |--1---*--| Id           PK |        |
| RegNo     (UQ) |         | DriverId     FK |        |
| Make           |         | VehicleId    FK |        |
| Model          |         | RouteId      FK |        |
| Year           |         | CustomerId   FK |  (nullable, SET NULL)
| Color          |         | CustomerName    |
| Capacity       |         | CustomerPhone   |
| FuelType       |         | CustomerEmail   |
| Status         |         | BookingDate     |
| Mileage        |         | TripDate        |
+-------+--------+         | Status          |
        |                   | Cost            |
        |                   +-----+-----+-----+
        |                         |     |
        1                         1     *
        |                         |     |
+-------+--------+         +-----+--+  +-----+------+
|  Maintenance   |         | Billing |  |  Expense   |
|    Record      |         +--------+   +------------+
+----------------+         | Id   PK |  | Id      PK |
| Id          PK |         | TripId FK|  | VehicleId FK|
| VehicleId   FK |         | Amount   |  | TripId   FK |
| Description    |         | PayDate  |  | Category    |
| Cost           |         | Method   |  | Amount      |
| Date           |         | Status   |  | Date        |
| NextMaintDate  |         +----------+  | Description |
| Status         |                       | ApprovedBy  |
+----------------+                       +-------------+

+----------------+         +----------------+
|     Route      |         |   AuditLog     |
+----------------+         +----------------+
| Id          PK |--1---*--Trip             | Id          PK |
| Origin         |                          | EntityName     |
| Destination    |                          | EntityId       |
| Distance       |                          | Action         |
| EstTimeHours   |                          | Changes (JSON) |
| BaseCost       |                          | UserId         |
+----------------+                          | UserName       |
                                            | Timestamp      |
+ ASP.NET Identity tables                  +----------------+
  (AspNetUsers, AspNetRoles,
   AspNetUserRoles, etc.)
```

### Referential Integrity Rules

| Relationship | Delete Behavior | Rationale |
|-------------|----------------|-----------|
| Employee -> Driver | Restrict | Prevent orphaned driver records |
| Driver -> Trip | Restrict | Prevent deletion of drivers with trip history |
| Vehicle -> Trip | Restrict | Prevent deletion of vehicles with trip history |
| Route -> Trip | Restrict | Prevent deletion of routes with trip history |
| Customer -> Trip | Set Null | Trips persist after customer deletion; CustomerId becomes null |
| Trip -> Billing | Cascade | Billing is meaningless without its trip |
| Trip -> Expense | Set Null | Expenses persist after trip deletion for financial records |
| Vehicle -> MaintenanceRecord | Cascade | Maintenance history follows vehicle lifecycle |
| Vehicle -> Expense | Set Null | Expenses persist after vehicle deletion for financial records |

---

## Modules & Access Control

### Admin Module
> Full system oversight, configuration, and customer management

| Feature | Description |
|---------|-------------|
| **Dashboard** | System-wide KPIs: vehicles, drivers, employees, active trips, users, revenue. Includes trip status and monthly revenue charts. Displays license expiry alerts for drivers within 30 days of expiration. |
| **User Management** | Create and manage system users; assign roles (Admin, FinanceManager, HRManager, TravelManager) |
| **Vehicle Management** | Fleet CRUD with search; track registration, capacity, fuel type, mileage, and availability status |
| **Route Management** | Define origin-destination routes with distance, estimated time, and base cost |
| **Customer Management** | Customer directory with CRUD, search, pagination, and CSV export. Details view shows trip history and total spend. |
| **Audit Logs** | Searchable audit trail of all create, update, and delete operations across the system |

### Finance Module
> Revenue tracking, expense management, and financial reporting

| Feature | Description |
|---------|-------------|
| **Dashboard** | Financial KPIs: total revenue, monthly revenue, pending/completed payments, total expenses, monthly expenses, net profit. Includes payment method, revenue trend, and expense category charts. |
| **Billing** | Create and manage billing records linked to trips; filter by payment status and date range. Download PDF invoices from billing details. |
| **Expense Tracking** | Record and manage expenses categorized by type (Fuel, Toll, Parking, etc.) with optional vehicle and trip associations. Filter by category, vehicle, and date range. CSV export supported. |
| **Reports** | Revenue summaries with payment method breakdown, period-based analysis, total expenses, and net profit/loss calculations |

### HR Module
> Workforce administration and compliance monitoring

| Feature | Description |
|---------|-------------|
| **Dashboard** | Workforce KPIs: total/active employees, available drivers, employees on leave. Displays license expiry alerts for drivers approaching or past license expiration. |
| **Employee Management** | Full CRUD with position (Driver, Receptionist, Mechanic, Cleaner) and status filters |
| **Driver Management** | Register drivers linked to employee records; track license numbers and expiry dates. Visual badges indicate expired or expiring-soon licenses. |

### Travel Module
> Trip operations and vehicle maintenance

| Feature | Description |
|---------|-------------|
| **Dashboard** | Operations KPIs: total/active/completed trips, vehicle availability, overdue maintenance count |
| **Trip Management** | Book trips with driver, vehicle, route, and optional customer selection. Automated status workflow updates driver/vehicle availability. Email confirmations sent to customers when configured. |
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
- Create the database and apply all EF Core migrations
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

> The Admin account can create additional users through the User Management module.

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

### Email Settings

Configure SMTP in `appsettings.json` to enable email notifications (password resets, booking confirmations):

```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "FromEmail": "noreply@cabmanagement.com",
    "FromName": "Cab Management System",
    "EnableSsl": true
  }
}
```

> When SMTP is not configured, the application logs a warning and continues without sending emails. No functionality is blocked.

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

### Security Headers

The following HTTP security headers are applied to all responses:

| Header | Value |
|--------|-------|
| X-Frame-Options | DENY |
| X-Content-Type-Options | nosniff |
| X-XSS-Protection | 1; mode=block |
| Referrer-Policy | strict-origin-when-cross-origin |
| Content-Security-Policy | Configured for self, inline styles/scripts, and CDN assets |

### Logging

Structured logging is enabled across all controllers and services using `ILogger<T>`. Log levels are configured in `appsettings.json`:

| Category | Level |
|----------|-------|
| Default | Information |
| Microsoft.AspNetCore | Warning |

---

## License

This project is developed for educational and demonstration purposes.
