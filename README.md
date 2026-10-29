# 🛒 E-Commerce REST API - Enterprise Clean Architecture

A production-grade, highly scalable E-Commerce backend API built from scratch using **ASP.NET Core (Minimal APIs / Controllers)**, implementing **Clean Architecture** principles, **CQRS**, and modern enterprise design patterns.

This project was built as a collaborative effort to simulate a real-world enterprise environment, focusing on maintainability, scalability, security, and high performance.

---

## 🏗️ Architecture & Design

The project strictly follows **Clean Architecture (N-Tier)** to ensure the separation of concerns and independence of frameworks/databases.

### Project Layers:
*   **Domain:** Contains Enterprise logic, Entities, Enums, and Repository Interfaces. (No dependencies).
*   **Application:** Contains Business Logic, DTOs, Interfaces, CQRS (Commands/Queries), Validators, and Mappings. (Depends only on Domain).
*   **Infrastructure:** Contains Data Access (EF Core), External Services (Stripe, SignalR, Redis, Cloudinary), and Repository Implementations. (Depends on Application).
*   **API (WebAPI):** The presentation layer containing Controllers, Middlewares, and DI configuration. (Depends on Application & Infrastructure).

### Key Design Patterns Used:
*   **Repository & Unit of Work:** To encapsulate data access and ensure atomic transactions.
*   **CQRS (Command Query Responsibility Segregation):** Implemented using **MediatR** in the Orders module to separate reads from writes.
*   **Pipeline Behaviors (MediatR):** Used for cross-cutting concerns (Validation, Logging, and Database Transactions) automatically applied to all Commands.
*   **Specification Pattern:** For dynamic filtering, sorting, and pagination in the Product Catalog.
*   **Options Pattern:** For strongly-typed configuration settings.
*   **Result/Exception Handling:** Global Exception Handling Middleware mapping custom exceptions (NotFound, BadRequest, Forbidden) to proper HTTP Status Codes.

---

## 🚀 Tech Stack & Features

### 🛠️ Technologies:
*   **Framework:** .NET 9 / ASP.NET Core Web API
*   **Database:** SQL Server + Entity Framework Core (Code-First)
*   **Caching:** Redis (for high-performance Shopping Cart)
*   **Real-time:** SignalR (for instant Notifications)
*   **Background Jobs:** Hangfire (for scheduled tasks & cleanups)
*   **Payment Gateway:** Stripe API (Supporting 3D Secure & Webhooks)
*   **Security:** ASP.NET Core Identity + JWT + Refresh Tokens
*   **Mapping:** AutoMapper
*   **Validation:** FluentValidation
*   **Logging:** Serilog + Seq (Structured Logging)
*   **Containerization:** Docker

### ✨ Core Features:
*   **🔐 Authentication & Authorization:** Secure JWT-based authentication with Role-based access (Admin, Customer) and Refresh Tokens.
*   **📦 Catalog Management:** Full CRUD for Products, Categories, and Brands. Supports image uploads (Cloudinary) and advanced Specification Pattern for searching, sorting, and pagination.
*   **🛒 Shopping Cart (Redis):** Lightning-fast cart operations using Redis, preventing DB bottlenecks.
*   **📋 Order Management (CQRS):**
    *   Create orders with complex business logic (stock deduction, coupon validation).
    *   **Inventory Management:** Uses strict Database Transactions and **Stock Reservations** to prevent Race Conditions (over-selling).
    *   **Snapshot Pattern:** Shipping address and product prices are snapshotted at the time of purchase to ensure invoice immutability.
*   **💳 Payments (Stripe):** Secure Stripe integration handling PaymentIntents, Webhooks for asynchronous payment confirmation, and Refunds.
*   **⭐ Reviews & Wishlist:**
    *   Unique reviews per user per product (enforced by DB Composite Unique Index).
    *   Auto-calculating Product `AverageRating` and `ReviewsCount` on-the-fly to optimize read performance.
    *   Fast Wishlist management.
*   **🔔 Real-time Notifications:** Hybrid approach: Saves notifications to DB (for offline users) and pushes them instantly via SignalR.
*   **⏱️ Background Jobs (Hangfire):** Automated cleanup of expired Stock Reservations (runs every 5 minutes) to release unreserved inventory back to the store.
*   **🌐 Global Audit Logging:** Auto-filling `CreatedAt`, `UpdatedAt`, `CreatedBy`, and `UpdatedBy` in the `SaveChangesAsync` interceptor. Soft-delete implementation.

---

## ⚙️ How to Run Locally

### Prerequisites:
*   [.NET 9 SDK](https://dotnet.microsoft.com/download)
*   [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
*   [Redis](https://redis.io/download) (Or run via Docker: `docker run -d -p 6379:6379 redis`)
*   [Stripe Account](https://stripe.com/) (For API Keys)

### Setup Steps:
1.  **Clone the repository:**
    ```bash
    git clone https://github.com/moaazalaa12/ECommerce-CleanArchitecture.git
    cd ECommerce.CleanArchitecture
