# Task[in] 2.0

Task[in] 2.0 is a productivity application backend built using ASP.NET Core Web API, following Clean Architecture principles. It provides RESTful APIs for managing projects, tasks, and Pomodoros, utilizing the Pomodoro Technique to enhance productivity.

## Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Technologies Used](#technologies-used)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Setup Instructions](#setup-instructions)
  - [Running the Application](#running-the-application)
- [API Endpoints](#api-endpoints)
- [Database Migrations](#database-migrations)
- [Contributing](#contributing)
- [License](#license)

## Features

- **Project Management**: Create, read, update, and delete projects.
- **Task Management**: CRUD operations for tasks within projects.
- **Pomodoro Management**: Manage Pomodoros associated with tasks.
- **CQRS and MediatR**: Implements Command and Query patterns for separation of concerns.
- **Entity Framework Core**: Uses EF Core for data access with SQL Server.
- **Clean Architecture**: Ensures a maintainable and testable codebase.

## Architecture

The backend follows the **Clean Architecture** pattern, dividing the solution into four projects:

1. **ElGuerre.Taskin.Api**: The presentation layer containing controllers.
2. **ElGuerre.Taskin.Application**: Contains application logic, commands, queries, and handlers.
3. **ElGuerre.Taskin.Domain**: Defines domain entities and interfaces.
4. **ElGuerre.Taskin.Infrastructure**: Implements data access and persistence using Entity Framework Core.

## Technologies Used

- **.NET 6 SDK**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **MediatR**
- **SQL Server**
- **Swagger** (for API documentation)

## Project Structure

```
ElGuerre.Taskin.sln
├── ElGuerre.Taskin.Api
│   ├── Controllers
│   ├── Program.cs
│   └── appsettings.json
├── ElGuerre.Taskin.Application
│   ├── Projects
│   ├── Tasks
│   └── Pomodoros
├── ElGuerre.Taskin.Domain
│   └── Entities
└── ElGuerre.Taskin.Infrastructure
    ├── EntityFramework
    │   ├── TaskinDbContext.cs
    │   └── Configurations
    └── Migrations
```

## Getting Started

### Prerequisites

- **.NET 6 SDK**: Download and install from [Microsoft .NET](https://dotnet.microsoft.com/download).
- **SQL Server**: Install SQL Server or use SQL Server Express.
- **Visual Studio 2022** or **Visual Studio Code**

### Setup Instructions

1. **Clone the Repository**

   ```bash
   git clone https://github.com/your-username/taskin-backend.git
   cd taskin-backend
   ```

2. **Restore NuGet Packages**

   Open the solution in Visual Studio or use the command line:

   ```bash
   dotnet restore
   ```

3. **Configure Database Connection**

   Update the `appsettings.json` file in `ElGuerre.Taskin.Api` with your SQL Server connection string:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=your_server;Database=TaskinDB;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }
   ```

4. **Apply Database Migrations**

   Navigate to the `back/src/` directory and run:

   ```bash
   # Add a migration
   dotnet ef migrations add InitialCreate --startup-project ElGuerre.Taskin.Api --project ElGuerre.Taskin.Infrastructure

   # Update the database
   dotnet ef database update --startup-project ElGuerre.Taskin.Api --project ElGuerre.Taskin.Infrastructure
   ```

   Ensure that the `ElGuerre.Taskin.Api` project is set as the startup project.

## Running the Application

1. **Start the API**

   Navigate to the `back/src/` directory and run:

   ```bash
   dotnet run --project ElGuerre.Taskin.Api
   ```

   The API will start and listen on `https://localhost:5001` by default.

2. **Test the API**

   Open a browser and navigate to `https://localhost:5001/swagger` to view the Swagger UI and test the endpoints.

## API Endpoints

### Projects

- **GET** `/api/Projects` - Get a list of projects.
- **GET** `/api/Projects/{id}` - Get a project by ID.
- **POST** `/api/Projects` - Create a new project.
- **PUT** `/api/Projects/{id}` - Update an existing project.
- **DELETE** `/api/Projects/{id}` - Delete a project.

### Tasks

- **GET** `/api/Tasks?projectId={projectId}` - Get tasks by project ID.
- **GET** `/api/Tasks/{id}` - Get a task by ID.
- **POST** `/api/Tasks` - Create a new task.
- **PUT** `/api/Tasks/{id}` - Update an existing task.
- **DELETE** `/api/Tasks/{id}` - Delete a task.

### Pomodoros

- **GET** `/api/Pomodoros?taskId={taskId}` - Get pomodoros by task ID.
- **GET** `/api/Pomodoros/{id}` - Get a pomodoro by ID.
- **POST** `/api/Pomodoros` - Create a new pomodoro.
- **PUT** `/api/Pomodoros/{id}` - Update an existing pomodoro.
- **DELETE** `/api/Pomodoros/{id}` - Delete a pomodoro.

## Database Migrations

To manage database schema changes, use Entity Framework Core migrations. Run these commands from the `back/src/` directory:

### Adding a Migration

```bash
dotnet ef migrations add MigrationName --startup-project ElGuerre.Taskin.Api --project ElGuerre.Taskin.Infrastructure -o EntityFramework/Migrations
```

### Updating the Database

```bash
dotnet ef database update --startup-project ElGuerre.Taskin.Api --project ElGuerre.Taskin.Infrastructure
```

## Contributing

Contributions are welcome! Please follow these steps:

1. **Fork the Repository**

   Click the "Fork" button at the top right of the repository page.

2. **Create a Feature Branch**

   ```bash
   git checkout -b feature/YourFeature
   ```

3. **Commit Your Changes**

   ```bash
   git commit -m "Add your message here"
   ```

4. **Push to Your Fork**

   ```bash
   git push origin feature/YourFeature
   ```

5. **Create a Pull Request**

   Open a pull request to the `main` branch of the original repository.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact

For questions or support, please open an issue on the [GitHub repository](https://github.com/your-username/taskin-backend/issues).

## Acknowledgments

- **ASP.NET Core**: [https://docs.microsoft.com/aspnet/core](https://docs.microsoft.com/aspnet/core)
- **Entity Framework Core**: [https://docs.microsoft.com/ef/core](https://docs.microsoft.com/ef/core)
- **MediatR**: [https://github.com/jbogard/MediatR](https://github.com/jbogard/MediatR)
- **Clean Architecture**: [https://github.com/jasontaylordev/CleanArchitecture](https://github.com/jasontaylordev/CleanArchitecture)
- **Swagger**: [https://swagger.io/](https://swagger.io/)

Thank you for using Task[in] 2.0 Backend! We hope this tool helps you build efficient and scalable applications.
