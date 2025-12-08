# TickSyncAPI

## 1. Project Overview

**TickSyncAPI** is a backend RESTful Web API built with ASP.NET Core (.NET 8) for a ticket booking system.
The API supports movie discovery, show listings, seat selection with real-time updates, booking initiation, payments, and user/admin management.

The system integrates:

* Role-based JWT authentication
* Movie data synchronization from TMDB
* Seat locking with WebSocket-based real-time updates
* Razorpay payment flow
* SQL Server persistence via Entity Framework Core

Frontend implementation is **not included** in this repository.

---

## 2. Features

Implemented features based strictly on the codebase:

### Authentication & Authorization

* User login with JWT token
* Role-based authorization (`user`, `admin`)
* Assign roles to users
* Password reset flow with email verification

  * Forgot password
  * Verify reset code
  * Reset password

### User Management

* Register users
* Get all users / single user
* Update user
* Delete user

### Movie Management

* Fetch movies with filters (language, genre, pagination)
* Trending movies (by popularity)
* Recommended movies (by rating & release date)
* Related movies
* Search movies with fuzzy matching
* CRUD operations for movies
* TMDB integration:

  * Sync movies
  * Sync languages
  * Update movie popularity

### Shows & Venues

* Get upcoming shows for a movie grouped by venue
* Seat pricing per show (regular / premium)

### Seat & Booking System

* Fetch latest seat layout for a show
* Lock seats temporarily (cached)
* Real-time seat updates via WebSockets
* Initiate booking
* Cancel booking
* Retrieve booking history
* Retrieve booking details

### Payment

* Create Razorpay order
* Handle Razorpay payment callback
* Store payment details and status

### Admin APIs

* Get entity counts (users, movies, venues, shows, seats)
* Fetch entity data dynamically (users, shows, movies, venues, seats, bookings, payments)

### Infrastructure

* Global exception handling middleware
* WebSocket middleware
* Swagger enabled in development
* CI pipeline with GitHub Actions for build verification

---

## 3. Tech Stack

**Backend**

* ASP.NET Core Web API (.NET 8)
* Entity Framework Core 9
* SQL Server
* WebSockets (native ASP.NET Core)

**Authentication**

* JWT Bearer Authentication

**Payments**

* Razorpay SDK

**Email**

* MailKit (SMTP)

**Security**

* BCrypt password hashing

**Dev & Tooling**

* Swagger / OpenAPI
* GitHub Actions CI
* Repomix (repository packing)

---

## 4. Installation

### Prerequisites

* .NET SDK 8.0+
* SQL Server
* Visual Studio 2022 / VS Code (optional)

### Steps

```bash
git clone <repository-url>
cd TickSyncAPI
dotnet restore
dotnet build
dotnet run
```

The API will start using the URLs defined in `launchSettings.json`.

---

## 5. Configuration

Configuration values are read from `appsettings.json` / environment variables.

### Required Configuration (Not Fully Provided)

Some required settings are **not specified** in the repository and must be supplied manually:

* Database connection string:

  ```json
  "ConnectionStrings": {
    "DefaultConnection": "Not specified"
  }
  ```

* JWT settings:

  ```json
  "Jwt": {
    "Issuer": "Not specified",
    "Audience": "Not specified",
    "Key": "Not specified"
  }
  ```

* Email settings:

  ```json
  "EmailSettings": {
    "SenderName": "Not specified",
    "SenderEmail": "Not specified",
    "SmtpServer": "Not specified",
    "Port": "Not specified",
    "Username": "Not specified",
    "Password": "Not specified"
  }
  ```

* Razorpay secrets:

  * Key / Secret: **Not specified**

---

## 6. Usage

### Run API

```bash
dotnet run
```

### Swagger UI

Available in development mode:

```
/swagger
```

### WebSocket Endpoint

```
/ws?showId={showId}
```

Used for real-time seat updates for a specific show.

### Authentication

* Login via `POST /api/Auth/login`
* Include JWT token in `Authorization` header:

```
Authorization: Bearer <token>
```

---

## 7. Project Structure

```
TickSyncAPI/
├── Controllers/       # API endpoints
├── Services/          # Business logic
├── Interfaces/        # Service contracts
├── Models/            # EF Core entities
├── Dtos/              # API request/response models
├── Middlewares/       # Exception & WebSocket middleware
├── HelperClasses/     # Utilities (WebSocket, Razorpay, exceptions)
├── DB/                # SQL scripts (schema & data)
├── Properties/        # Launch settings
├── Program.cs         # Application bootstrap
```

---

## 8. Development Notes

* Entity Framework Core is used with database-first scaffolding (`Scaffold-DbContext`).
* Seat locking relies on in-memory caching and WebSocket broadcasts.
* Exception handling uses a centralized middleware with custom exceptions.
* CI pipeline validates build on pull requests to `main`.
* Authorization policies default to JWT.
* Some commented SQL scripts indicate experimental or development-only utilities.

---

## 9. License

**Not specified**
