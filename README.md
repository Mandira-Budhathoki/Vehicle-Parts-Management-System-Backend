# 🚗 Vehicle Parts Management System — Backend

<p align="center">
  <img src="https://img.shields.io/badge/ASP.NET_Core_8.0-%23512BD4.svg?style=for-the-badge&logo=dotnet&logoColor=white"/>
  <img src="https://img.shields.io/badge/C%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white"/>
  <img src="https://img.shields.io/badge/SQL_Server-%23CC2927.svg?style=for-the-badge&logo=microsoft-sql-server&logoColor=white"/>
  <img src="https://img.shields.io/badge/Swagger-%2385EA2D.svg?style=for-the-badge&logo=swagger&logoColor=black"/>
</p>

> **RESTful API backend** for the Vehicle Parts Management System — built with ASP.NET Core 8.0 using a clean, layered architecture.

🔗 **Frontend Repo**: [Vehicle-Parts-Management-System---Frontend](https://github.com/Mandira-Budhathoki/Vehicle-Parts-Management-System---Frontend)

---

## 📌 Project Overview

This is the **ASP.NET Core 8.0 Web API** powering the Vehicle Parts Management System. It handles authentication, inventory management, sales invoicing, vendor management, appointment booking, customer loyalty, email notifications, and financial reporting — all via a structured RESTful API.

---

## 🏗️ Architecture

The backend follows a clean, layered architecture pattern:

```
Request → Controller → Interface → Service → Data (EF Core) → SQL Server
```

| Layer | Folder | Responsibility |
|---|---|---|
| **Controllers** | `Controllers/` | Handle HTTP requests, route to services |
| **Services** | `Services/` | Business logic implementation |
| **Interfaces** | `Interfaces/` | Service contracts (dependency inversion) |
| **Models** | `Model/` | Entity classes (EF Core models) |
| **DTOs** | `Dto/` | Data Transfer Objects for API input/output |
| **Data** | `Data/` | DbContext, EF Core configuration |
| **Migrations** | `Migrations/` | EF Core database migrations |

---

## 🛠️ Tech Stack

- **Framework**: ASP.NET Core 8.0
- **Language**: C#
- **ORM**: Entity Framework Core
- **Database**: SQL Server
- **Authentication**: JWT Bearer Tokens
- **API Docs**: Swagger / OpenAPI
- **Email**: SMTP Email Service

---

## 📁 Project Structure

```
Vehicle-Parts-Management-System-Backend/
├── Controllers/
│   ├── AuthController.cs              # Login, register, JWT auth
│   ├── AdminController.cs             # Admin-level operations
│   ├── PartsController.cs             # Vehicle parts CRUD
│   ├── VendorController.cs            # Supplier/vendor management
│   ├── StaffController.cs             # Staff management
│   ├── CustomerController.cs          # Customer management
│   ├── SalesController.cs             # Sales invoice creation
│   ├── SalesHistoryController.cs      # Sales history & records
│   ├── PurchaseInvoicesController.cs  # Purchase invoice management
│   ├── AppointmentController.cs       # Appointment booking
│   ├── PartRequestController.cs       # Customer part requests
│   ├── ReviewController.cs            # Customer reviews
│   ├── ReportsController.cs           # Financial & staff reports
│   ├── NotificationController.cs      # Admin notifications
│   ├── EmailController.cs             # Email dispatch service
│   └── VehicleController.cs           # Vehicle records
├── Model/
│   ├── User.cs, Part.cs, Vendor.cs    # Core entities
│   ├── Sales.cs, SalesItem.cs         # Sales models
│   ├── Purchase.cs, PurchaseItem.cs   # Purchase models
│   ├── Appointment.cs                 # Booking model
│   ├── PartRequest.cs, Review.cs      # Customer interaction models
│   ├── LoyaltyTier.cs                 # Loyalty programme model
│   ├── Notification.cs, Payment.cs    # Support models
│   └── Vehicle.cs                     # Vehicle model
├── Services/                          # Business logic implementations
├── Interfaces/                        # Service contracts
├── Dto/                               # Request/Response DTOs
├── Data/                              # EF Core DbContext
├── Migrations/                        # Database migrations
├── Program.cs                         # App entry point & DI setup
└── appsettings.json                   # Configuration (DB, JWT, SMTP)
```

---

## ⚙️ Getting Started

### Prerequisites
- **.NET 8 SDK**
- **SQL Server** (local or remote)
- **Visual Studio 2022** or VS Code with C# extension

### Installation & Run

1. **Clone the repository**
   ```bash
   git clone https://github.com/Mandira-Budhathoki/Vehicle-Parts-Management-System-Backend.git
   cd Vehicle-Parts-Management-System-Backend
   ```

2. **Configure the connection string** in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=VehiclePartsDB;Trusted_Connection=True;"
     },
     "Jwt": {
       "Key": "YOUR_SECRET_KEY",
       "Issuer": "VehiclePartsAPI"
     }
   }
   ```

3. **Apply database migrations**:
   ```bash
   dotnet ef database update
   ```

4. **Run the application**:
   ```bash
   dotnet restore
   dotnet run
   ```

5. **Access Swagger UI** at: `https://localhost:5001/swagger`

---

## 🔌 API Endpoints Overview

| Module | Base Route | Description |
|---|---|---|
| Auth | `/api/auth` | Login, register, JWT token |
| Parts | `/api/parts` | Vehicle parts inventory CRUD |
| Vendors | `/api/vendor` | Supplier management |
| Staff | `/api/staff` | Staff accounts & roles |
| Customers | `/api/customer` | Customer management |
| Sales | `/api/sales` | Sales invoices & point of sale |
| Purchases | `/api/purchaseinvoices` | Purchase invoice tracking |
| Appointments | `/api/appointment` | Booking management |
| Part Requests | `/api/partrequest` | Customer part requests |
| Reviews | `/api/review` | Customer reviews |
| Reports | `/api/reports` | Financial & operational reports |
| Notifications | `/api/notification` | Admin notifications |
| Email | `/api/email` | Email dispatch |
| Vehicles | `/api/vehicle` | Vehicle records |

---

## 👥 Team

This was a collaborative group project developed as part of a BSc Computing module.

---

## 👩‍💻 Author

**Mandira Budhathoki**
📧 [mandirabudhathoki091@gmail.com](mailto:mandirabudhathoki091@gmail.com)
🔗 [LinkedIn](https://www.linkedin.com/in/mandira-budhathoki-8077a0338/)
🐙 [GitHub](https://github.com/Mandira-Budhathoki)
