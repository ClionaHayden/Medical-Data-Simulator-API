# Medical API Simulator

A RESTful API built with ASP.NET Core for managing patient vitals, medical devices, and patient records.  
Includes JWT authentication, role-based authorization, XML documentation, and a background service simulating real-time vital sign data.

## Features

- CRUD operations for Patients, Medical Devices, and Vitals
- JWT authentication and role-based access control (Admin/User roles)
- Pagination support on GET endpoints
- XML API documentation enabled for Swagger
- Background service generating simulated vital data every 10 seconds
- Unit and integration tests for controllers

## Technologies

- ASP.NET Core Web API (.NET 9)
- Entity Framework Core with SQL Server
- JWT Authentication
- Swagger / OpenAPI
- xUnit and Moq for testing

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- SQL Server or SQL Server Express
- (Optional) VS Code or Visual Studio

### Setup

1. Clone the repository:

   ```bash
   git clone https://github.com/ClionaHayden/Medical-Data-Simulator-API.git
    ```
2. Configure the database connection in `appsettings.json`:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=MedicalSimDb;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```
3. Configure JWT settings in `appsettings.json`:

   ```json
   "Jwt": {
   "Key": "your-very-secure-secret-key",
   "Issuer": "MedicalApiSimulator",
   "Audience": "MedicalApiUsers"
   "ExpireMinutes": 60
   }
    ```
4. Run database migrations and seed data:

   ```bash
   dotnet ef database update
    ```
5. Run the application:

   ```bash
   dotnet run
    ```
6. Access Swagger UI at `https://localhost:5001/swagger` or Swagger's listening port to explore the API endpoints. This can be found by finding a line similar to below when the application runs:
    ```bash
     info: Microsoft.Hosting.Lifetime[14]
        Now listening on: http://localhost:5164
    ```
## Authentication
  - Use `/api/auth/login` to obtain a JWT token.
  
  - Sample credentials:
    - Admin: username `doctor` / password `med123`
    - User: username `user` / password `userpass`
    
  - Include the JWT token in the `Authorization` header as:
    `Bearer YOUR_TOKEN`
  
## Features
- Manage Patients, Medical Devices, and Vitals with CRUD operations.
- Role-based authorization to restrict creation, update, and deletion to Admin users.
- Background service generating simulated vital data periodically.
- Paging support on GET endpoints for Patients and Vitals.
- XML comments enabled for Swagger API documentation.

## Running Tests
From the test project folder (`MedicalApiSimulator.Tests`):
   ```bash
   dotnet run
```

Tests cover:
- PatientsController CRUD and validation.
- Authentication scenarios.
- Paging functionality.

## License
This project is licensed under the [MIT License](LICENSE)  - see the LICENSE file for details.

## Contact
Created by Cliona Hayden - feel free to reach out!
