<div align="center">

# 🚖 Cab Management System

### Enterprise-Grade Fleet & Operations Management Platform

**Built with ASP.NET Core 8 MVC | Entity Framework Core | Bootstrap 5 | Chart.js**

<br/>

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/EF%20Core-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://learn.microsoft.com/en-us/ef/core/)
[![Bootstrap 5](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white)](https://getbootstrap.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-LocalDB-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
[![Chart.js](https://img.shields.io/badge/Chart.js-4.4-FF6384?style=for-the-badge&logo=chartdotjs&logoColor=white)](https://www.chartjs.org/)

<br/>

[![MailKit](https://img.shields.io/badge/MailKit-4.3-2196F3?style=flat-square)](https://github.com/jstedfast/MailKit)
[![QuestPDF](https://img.shields.io/badge/QuestPDF-2024.3-FF6F00?style=flat-square)](https://www.questpdf.com/)
[![Bootstrap Icons](https://img.shields.io/badge/Bootstrap%20Icons-1.11-7952B3?style=flat-square)](https://icons.getbootstrap.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-22c55e?style=flat-square)](LICENSE)

<br/>

*A unified, role-based platform for managing fleet operations, trip booking, customer self-service, driver performance ratings, billing, expense tracking, HR, and vehicle maintenance — complete with dark mode support.*

</div>

<br/>

---

## 📋 Table of Contents

<details>
<summary>Click to expand</summary>

- [Overview](#-overview)
- [Key Features](#-key-features)
- [Technology Stack](#-technology-stack)
- [Architecture](#-architecture)
- [Project Structure](#-project-structure)
- [Database Design](#-database-design)
- [Modules & Access Control](#-modules--access-control)
- [Getting Started](#-getting-started)
- [Default Credentials](#-default-credentials)
- [Configuration](#-configuration)
- [License](#-license)

</details>

---

## 🔍 Overview

The **Cab Management System** is a comprehensive web application designed to digitize and streamline the end-to-end operations of a cab or fleet agency. It provides dedicated workspaces for **five operational roles**:

| Role | Area | Purpose |
|:-----|:-----|:--------|
| **Admin** | System-wide | Fleet, routes, customers, users, audit logs |
| **Finance Manager** | Financial | Billing, expenses, revenue reports, invoices |
| **HR Manager** | Workforce | Employees, drivers, license compliance, performance |
| **Travel Manager** | Operations | Trips, maintenance, ratings |
| **Customer** | Self-Service | Trip booking, invoices, profile management |

The platform covers the **complete operational lifecycle**: customer self-service registration and trip booking, driver performance tracking with star ratings, employee and driver management, vehicle and route maintenance, payment processing, expense tracking, PDF invoice generation, license compliance monitoring, email notifications, and a system-wide **dark mode** for user comfort.

---

## ✨ Key Features

### 🏢 Core Operations
| Feature | Description |
|:--------|:------------|
| Multi-Role Dashboards | 5 dedicated dashboards with role-specific KPIs, Chart.js visualizations, and contextual alerts |
| Full CRUD Operations | Create, view, edit, and delete across all 10 entity types with server-side validation |
| Advanced Search & Filtering | Text search, enum dropdowns, date ranges, and multi-criteria filtering on all list views |
| Server-Side Pagination | Consistent pagination across all list views with query string preservation |

### 👤 Customer Self-Service Portal
| Feature | Description |
|:--------|:------------|
| Customer Registration | Public sign-up form creating Identity user + Customer profile with auto-login |
| Personalized Dashboard | Trip count, active trips, total spend, recent trips, and quick action buttons |
| Self-Service Trip Booking | Select route and date; driver and vehicle are auto-assigned from available resources |
| Trip History & Details | Full trip list with status badges, detailed view with visual progress tracker |
| Invoice Access | Download PDF invoices directly from the customer portal |
| Profile Management | View and edit personal information (name, phone, address) |

### ⭐ Driver Performance & Ratings
| Feature | Description |
|:--------|:------------|
| Trip Rating System | Interactive 5-star rating with hover effects on completed trips, one rating per trip |
| Performance Dashboard | Per-driver analytics: average rating, total trips, completion rate, revenue, charts |
| Top Rated Drivers | Ranked leaderboard on HR Dashboard with trophy badges and performance links |
| Visual Star Display | Star ratings rendered inline on tables and detail views throughout the app |

### 🚕 Customer & Trip Management
| Feature | Description |
|:--------|:------------|
| Customer Profiles | Centralized directory with contact details, trip history, and total spend tracking |
| Trip Booking Workflow | Automated driver/vehicle status transitions through trip states (Pending → Completed) |
| Customer-Trip Linking | Optional association between customer profiles and bookings for repeat tracking |

### 💰 Financial Management
| Feature | Description |
|:--------|:------------|
| Billing & Payments | Payment records linked to trips with status tracking, method breakdown, and date filtering |
| Expense Tracking | Categorized recording (Fuel, Toll, Parking, Driver Allowance, Insurance, Other) |
| Profit & Loss Analysis | Real-time net profit calculations on Finance dashboard and Reports page |
| Revenue Reporting | Monthly and total summaries with payment method distribution and trend analysis |
| PDF Invoice Generation | Professional A4 invoices via QuestPDF with company branding and billing summary |

### 👥 HR & Compliance
| Feature | Description |
|:--------|:------------|
| Employee Management | Workforce management with position classification and employment status tracking |
| Driver Management | Driver registration with license number and expiry date tracking |
| License Expiry Alerts | Proactive alerts on Admin and HR dashboards for licenses expiring within 30 days |
| Compliance Badges | "Expired" and "Expiring Soon" badges on Driver Index for at-a-glance monitoring |

### 📧 Communication
| Feature | Description |
|:--------|:------------|
| Email Notifications | MailKit-powered transactional email with graceful degradation when SMTP is unconfigured |
| Password Reset Emails | Secure token-based password reset flow with email delivery |
| Booking Confirmations | Automated email confirmations sent to customers upon trip creation |

### 🔧 Platform Capabilities
| Feature | Description |
|:--------|:------------|
| Role-Based Authorization | ASP.NET Identity with area-level route protection and per-controller enforcement |
| Comprehensive Audit Trail | Automatic change tracking recording entity name, action, changed fields, and user |
| CSV Data Export | One-click export on Customers, Vehicles, Trips, Billings, and Expenses |
| 🌙 Dark Mode | System-wide theme with CSS variables, navbar toggle, localStorage, system preference detection, smooth transitions, and Chart.js color updates |
| Security Headers | X-Frame-Options, X-Content-Type-Options, X-XSS-Protection, Referrer-Policy, CSP |
| Structured Logging | `ILogger<T>` throughout all controllers and services with semantic templates |
| Auto-Migration & Seeding | Database schema and sample data provisioned automatically on first run |
| Responsive Design | Bootstrap 5 layout with collapsible sidebar, mobile-friendly tables, and badges |

---

## 🛠 Technology Stack

| Layer | Technology | Version | Purpose |
|:------|:-----------|:--------|:--------|
| **Runtime** | .NET | 8.0 | Cross-platform application framework |
| **Web Framework** | ASP.NET Core MVC | 8.0 | Server-side request handling and view rendering |
| **ORM** | Entity Framework Core | 8.0 | Object-relational mapping, migrations, change tracking |
| **Database** | SQL Server | LocalDB | Relational data storage |
| **Authentication** | ASP.NET Core Identity | 8.0 | User management, password hashing, role-based claims |
| **Email** | MailKit | 4.3 | SMTP email delivery for notifications |
| **PDF** | QuestPDF | 2024.3 | Fluent API for professional PDF invoices |
| **Frontend** | Razor Views + Bootstrap | 5.3 | Server-rendered HTML with responsive CSS |
| **Charts** | Chart.js | 4.4 | Interactive visualizations (pie, bar, line) |
| **Icons** | Bootstrap Icons | 1.11 | UI iconography via CDN |
| **Validation** | jQuery Validation | Unobtrusive | Client-side form validation |

---

## 🏗 Architecture

The application implements a **layered architecture** with clear separation of concerns:

```
┌─────────────────────────────────────────┐
│            Presentation Layer            │
│    Razor Views  ·  Bootstrap 5           │
│    Chart.js  ·  Dark Mode  ·  Tag Helpers│
├─────────────────────────────────────────┤
│            Controller Layer              │
│    5 Area Controllers  ·  [Authorize]    │
│    ViewModels  ·  TempData               │
├─────────────────────────────────────────┤
│             Service Layer                │
│    Business Logic  ·  Orchestration      │
│    Email  ·  PDF  ·  Dashboard Stats     │
├─────────────────────────────────────────┤
│            Repository Layer              │
│    Generic CRUD  ·  Eager Loading        │
│    Custom Queries  ·  Filtering          │
├─────────────────────────────────────────┤
│      EF Core + ASP.NET Identity          │
│    DbContext  ·  Migrations              │
│    Audit Interceptor  ·  Seeding         │
├─────────────────────────────────────────┤
│             SQL Server                   │
└─────────────────────────────────────────┘
```

### Design Principles

| Principle | Implementation |
|:----------|:---------------|
| **Generic Repository** | `IRepository<T>` / `Repository<T>` provides CRUD, counting, and predicate queries; entity repos extend with custom methods |
| **Thin Controllers** | Controllers delegate to services; no direct repository or DbContext access |
| **Area Isolation** | Each module (Admin, Finance, HR, Travel, CustomerPortal) in its own MVC Area |
| **ViewModel Separation** | Domain models never bind directly to forms; ViewModels handle all form concerns |
| **Automatic Auditing** | `SaveChangesAsync` override sets audit timestamps and logs all changes to AuditLog |
| **Graceful Degradation** | Email and external services wrapped in try-catch to prevent core operation disruptions |

---

## 📁 Project Structure

```
Cab Management System/
│
├── Areas/
│   ├── Admin/                        # System administration
│   │   ├── Controllers/              # Dashboard, UserManagement, Vehicle, Route, Customer
│   │   └── Views/                    # 21 views
│   ├── CustomerPortal/               # Customer self-service
│   │   ├── Controllers/              # Dashboard, Trip, Invoice, Profile
│   │   └── Views/                    # 9 views
│   ├── Finance/                      # Financial management
│   │   ├── Controllers/              # Dashboard, Billing, Expense, Reports
│   │   └── Views/                    # 17 views
│   ├── HR/                           # Human resources
│   │   ├── Controllers/              # Dashboard, Employee, Driver
│   │   └── Views/                    # 13 views
│   └── Travel/                       # Trip operations
│       ├── Controllers/              # Dashboard, Trip, Maintenance
│       └── Views/                    # 13 views
│
├── Controllers/
│   ├── AccountController.cs          # Auth, registration, profile, password reset
│   └── HomeController.cs             # Landing page with role-based redirect
│
├── Data/
│   ├── ApplicationDbContext.cs        # DbContext, entity config, audit interceptor
│   └── DbSeeder.cs                   # Role, user, and sample data seeding
│
├── Helpers/
│   └── CsvExportHelper.cs            # Generic CSV export utility
│
├── Models/
│   ├── Enums/                         # 10 enums (DriverStatus, TripStatus, etc.)
│   ├── ViewModels/                    # 17 ViewModels
│   ├── BaseEntity.cs                  # Abstract base with audit timestamps
│   └── [10 domain models]            # Employee, Driver, Vehicle, Route, Trip,
│                                      # Billing, MaintenanceRecord, Customer,
│                                      # Expense, DriverRating
│
├── Repositories/
│   ├── IRepository.cs                 # Generic repository interface
│   ├── Repository.cs                  # Generic repository implementation
│   └── [10 entity repositories]       # Interface + implementation pairs
│
├── Services/
│   ├── EmailService.cs                # MailKit SMTP implementation
│   ├── InvoicePdfService.cs           # QuestPDF invoice implementation
│   ├── DashboardService.cs            # Multi-role dashboard statistics
│   └── [10 entity services]           # Business logic implementations
│
├── Views/
│   ├── Account/                       # Login, Register, Profile, Password flows
│   ├── Home/                          # Landing page
│   └── Shared/                        # _Layout, _Sidebar, _Login, _Pagination
│
├── wwwroot/
│   ├── css/
│   │   ├── site.css                   # Core styles with CSS variables
│   │   └── dark-mode.css              # Dark theme definitions & overrides
│   └── js/
│       └── site.js                    # Sidebar highlighting & theme toggle
│
├── Migrations/                        # EF Core migration files
├── Program.cs                         # Entry point, DI, middleware pipeline
└── appsettings.json                   # Connection strings, email, logging
```

---

## 🗄 Database Design

### Entity Relationship Diagram

```
┌────────────────┐       ┌────────────────┐       ┌────────────────┐
│    Employee     │       │     Driver     │       │    Customer    │
├────────────────┤       ├────────────────┤       ├────────────────┤
│ Id          PK │──1─1──│ Id          PK │       │ Id          PK │
│ Name           │       │ EmployeeId  FK │       │ Name           │
│ Email     (UQ) │       │ LicenseNo (UQ) │       │ Email     (UQ) │
│ Phone          │       │ LicenseExpiry  │       │ Phone          │
│ Position       │       │ Status         │       │ Address        │
│ Status         │       └───────┬────────┘       └───────┬────────┘
│ HireDate       │               │                        │
│ Salary         │               1                    0..* (optional)
└────────────────┘               │                        │
                                 *                        │
┌────────────────┐       ┌──────┴─────────┐◄──────────────┘
│    Vehicle     │       │      Trip      │
├────────────────┤       ├────────────────┤
│ Id          PK │──1─*──│ Id          PK │
│ RegNo     (UQ) │       │ DriverId    FK │
│ Make           │       │ VehicleId   FK │
│ Model          │       │ RouteId     FK │
│ Year           │       │ CustomerId  FK │  (nullable)
│ Color          │       │ CustomerName   │
│ Capacity       │       │ CustomerPhone  │
│ FuelType       │       │ BookingDate    │
│ Status         │       │ TripDate       │
│ Mileage        │       │ Status         │
└───────┬────────┘       │ Cost           │
        │                └──┬──────┬──────┘
        │                   │      │
        1                   1      *
        │                   │      │
┌───────┴────────┐   ┌─────┴──┐  ┌┴───────────┐  ┌────────────────┐
│  Maintenance   │   │Billing │  │  Expense   │  │ DriverRating   │
│    Record      │   ├────────┤  ├────────────┤  ├────────────────┤
├────────────────┤   │ Id  PK │  │ Id      PK │  │ Id          PK │
│ Id          PK │   │TripId FK│  │VehicleId FK│  │ TripId  FK(UQ) │
│ VehicleId   FK │   │ Amount  │  │ TripId  FK │  │ DriverId    FK │
│ Description    │   │PayDate  │  │ Category   │  │ Rating   (1-5) │
│ Cost           │   │ Method  │  │ Amount     │  │ Comment        │
│ Date           │   │ Status  │  │ Date       │  │ CustomerName   │
│ NextMaintDate  │   └────────┘  │ Description │  └────────────────┘
│ Status         │               │ ApprovedBy  │
└────────────────┘               └────────────┘

┌────────────────┐       ┌────────────────┐
│     Route      │       │   AuditLog     │
├────────────────┤       ├────────────────┤
│ Id          PK │──1─*──Trip             │ Id          PK │
│ Origin         │                        │ EntityName     │
│ Destination    │                        │ EntityId       │
│ Distance       │                        │ Action         │
│ EstTimeHours   │                        │ Changes (JSON) │
│ BaseCost       │                        │ UserId         │
└────────────────┘                        │ Timestamp      │
                                          └────────────────┘
+ ASP.NET Identity tables (AspNetUsers, AspNetRoles, AspNetUserRoles, etc.)
```

### Referential Integrity Rules

| Relationship | Delete Behavior | Rationale |
|:-------------|:---------------|:----------|
| Employee → Driver | **Restrict** | Prevent orphaned driver records |
| Driver → Trip | **Restrict** | Preserve trip history |
| Vehicle → Trip | **Restrict** | Preserve trip history |
| Route → Trip | **Restrict** | Preserve trip history |
| Customer → Trip | **Set Null** | Trips persist; `CustomerId` becomes null |
| Trip → Billing | **Cascade** | Billing follows trip lifecycle |
| Trip → Expense | **Set Null** | Expenses persist for financial records |
| Trip → DriverRating | **Cascade** | Rating follows trip lifecycle; unique TripId constraint |
| Driver → DriverRating | **Restrict** | Preserve rating history |
| Vehicle → Maintenance | **Cascade** | Maintenance follows vehicle lifecycle |
| Vehicle → Expense | **Set Null** | Expenses persist for financial records |

---

## 🔐 Modules & Access Control

### 🛡 Admin Module
> *Full system oversight, configuration, and customer management*

| Feature | Description |
|:--------|:------------|
| **Dashboard** | System-wide KPIs with trip status charts, monthly revenue, and license expiry alerts |
| **User Management** | Create and manage users; assign roles (Admin, Finance, HR, Travel) |
| **Vehicle Management** | Fleet CRUD with search; registration, capacity, fuel type, mileage tracking |
| **Route Management** | Origin-destination routes with distance, estimated time, and base cost |
| **Customer Management** | Customer directory with CRUD, search, pagination, CSV export, and trip history |
| **Audit Logs** | Searchable trail of all create, update, and delete operations |

### 💳 Finance Module
> *Revenue tracking, expense management, and financial reporting*

| Feature | Description |
|:--------|:------------|
| **Dashboard** | Revenue, expenses, net profit KPIs with payment method, trend, and expense charts |
| **Billing** | Manage billing records; filter by status and date range; download PDF invoices |
| **Expense Tracking** | Categorized expenses with vehicle/trip associations; filter and CSV export |
| **Reports** | Revenue summaries, payment breakdowns, period analysis, and profit/loss |

### 👨‍💼 HR Module
> *Workforce administration, compliance monitoring, and driver performance*

| Feature | Description |
|:--------|:------------|
| **Dashboard** | Workforce KPIs, license expiry alerts, and Top Rated Drivers leaderboard |
| **Employee Management** | Full CRUD with position and status filters |
| **Driver Management** | License tracking with visual compliance badges and performance links |
| **Driver Performance** | Analytics dashboard: average rating, trips, completion rate, revenue, charts |

### 🗺 Travel Module
> *Trip operations, vehicle maintenance, and trip ratings*

| Feature | Description |
|:--------|:------------|
| **Dashboard** | Trip and vehicle status KPIs with pie chart visualizations |
| **Trip Management** | Book trips with automated driver/vehicle assignment and email confirmations |
| **Trip Rating** | Interactive 5-star rating on completed trips with comments; one per trip |
| **Maintenance** | Schedule and track vehicle maintenance; flag overdue records |

### 🧑‍💻 Customer Portal
> *Self-service trip booking, tracking, and invoice management*

| Feature | Description |
|:--------|:------------|
| **Registration** | Public sign-up creating Identity user + Customer profile with auto-login |
| **Dashboard** | Personalized stats, recent trips table, and quick action buttons |
| **Trip Booking** | Select route and date; driver/vehicle auto-assigned; trip created as Pending |
| **My Trips** | Trip history with status badges, detailed view with progress tracker |
| **Invoices** | Billing list with PDF download buttons for each invoice |
| **Profile** | View and edit personal information |

---

## 🚀 Getting Started

### Prerequisites

| Requirement | Version |
|:------------|:--------|
| [.NET SDK](https://dotnet.microsoft.com/download/dotnet/8.0) | 8.0 or later |
| SQL Server | LocalDB (included with Visual Studio) or any instance |

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

> **Note:** The application will automatically create the database, apply all EF Core migrations, and seed roles, default users, and sample data on first run.

### Access the Application

| Protocol | URL |
|:---------|:----|
| HTTPS | `https://localhost:7188` |
| HTTP | `http://localhost:5195` |

### Custom Database Connection

Update `appsettings.json` to use a different SQL Server instance:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=CabManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

---

## 🔑 Default Credentials

The application seeds the following accounts on first run:

| Role | Email | Password |
|:-----|:------|:---------|
| **Admin** | `admin@cabsystem.com` | `Admin@123` |
| **Finance Manager** | `finance@cabsystem.com` | `Finance@123` |
| **HR Manager** | `hr@cabsystem.com` | `HR@1234` |
| **Travel Manager** | `travel@cabsystem.com` | `Travel@123` |

> **Tip:** The Admin account can create additional users through User Management. Customers can self-register via the **Register** link on the login page.

### Sample Data

The seeder provisions realistic sample data for immediate testing:

| Entity | Count | Notes |
|:-------|:------|:------|
| Employees | 8 | 4 drivers, 2 receptionists, 1 mechanic, 1 cleaner |
| Drivers | 4 | Linked to driver-position employees |
| Vehicles | 6 | Mixed fuel types and statuses |
| Routes | 5 | Indian city-to-city routes |
| Trips | 6 | Various statuses (Pending through Completed) |
| Billings | 4 | Pending and completed payments |
| Maintenance | 3 | Scheduled, in-progress, and completed |

---

## ⚙ Configuration

### Email Settings

Configure SMTP in `appsettings.json` to enable email notifications:

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

### Security Configuration

<details>
<summary><strong>Password Policy</strong></summary>

| Rule | Value |
|:-----|:------|
| Minimum length | 6 characters |
| Require uppercase | Yes |
| Require lowercase | Yes |
| Require digit | Yes |
| Require special character | Yes |

</details>

<details>
<summary><strong>Authentication</strong></summary>

| Setting | Value |
|:--------|:------|
| Login path | `/Account/Login` |
| Access denied path | `/Account/AccessDenied` |
| Session expiry | 30 days (sliding) |

</details>

<details>
<summary><strong>Security Headers</strong></summary>

| Header | Value |
|:-------|:------|
| X-Frame-Options | `DENY` |
| X-Content-Type-Options | `nosniff` |
| X-XSS-Protection | `1; mode=block` |
| Referrer-Policy | `strict-origin-when-cross-origin` |
| Content-Security-Policy | Configured for self, inline styles/scripts, and CDN assets |

</details>

<details>
<summary><strong>Logging</strong></summary>

Structured logging via `ILogger<T>` across all controllers and services:

| Category | Level |
|:---------|:------|
| Default | Information |
| Microsoft.AspNetCore | Warning |

</details>

---

## 📄 License

This project is developed for educational and demonstration purposes.

---

<div align="center">

**Made with ❤️ using ASP.NET Core 8**

</div>
