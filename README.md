# Task[in] 2.0

Task[in] 2.0 is a full-stack productivity application that implements the Pomodoro Technique for task management. The project consists of a modern Angular 19 frontend and a robust ASP.NET Core (.NET 9) backend following Clean Architecture principles.

## Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Technologies Used](#technologies-used)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Setup Instructions](#setup-instructions)
  - [Running the Application](#running-the-application)
- [Frontend (Angular)](#frontend-angular)
- [Backend (ASP.NET Core)](#backend-aspnet-core)
- [API Endpoints](#api-endpoints)
- [Database Migrations](#database-migrations)
- [Contributing](#contributing)
- [License](#license)

## Features

### Frontend Features
- **Modern Angular 19**: Standalone components with signal-based state management
- **Angular Material UI**: Responsive Material Design components
- **Multilingual Support**: Internationalization with Transloco (en-US, es-ES, zh-CN, zh-TW)
- **Real-time Updates**: Signal-based reactive state management with NgRx Signals
- **Progressive Web App**: Optimized for both desktop and mobile devices

### Backend Features
- **Project Management**: Create, read, update, and delete projects
- **Task Management**: CRUD operations for tasks within projects
- **Pomodoro Management**: Manage Pomodoros associated with tasks
- **CQRS and MediatR**: Implements Command and Query patterns for separation of concerns
- **Entity Framework Core**: Uses EF Core for data access with SQL Server
- **Clean Architecture**: Ensures a maintainable and testable codebase

### Core Domain
- **Projects**: Top-level containers for organizing work
- **Tasks**: Individual work items within projects
- **Pomodoros**: Time tracking sessions associated with tasks

## Architecture

Task[in] 2.0 follows modern architectural patterns to ensure maintainability, scalability, and testability:

### Frontend Architecture (Angular 19)
- **Component-Based**: Standalone components with signal-based inputs/outputs
- **State Management**: NgRx Signal Store for reactive state management
- **Feature-Based Structure**: Domain-organized modules with smart/dumb component pattern
- **Lazy Loading**: Route-based code splitting for optimal performance

### Backend Architecture (Clean Architecture)
The backend follows the **Clean Architecture** pattern, dividing the solution into four layers:

1. **API Layer** (`ElGuerre.Taskin.Api`): Controllers, middleware, and startup configuration
2. **Application Layer** (`ElGuerre.Taskin.Application`): Business logic, CQRS patterns with MediatR
3. **Domain Layer** (`ElGuerre.Taskin.Domain`): Core entities and domain logic
4. **Infrastructure Layer** (`ElGuerre.Taskin.Infrastructure`): Data access with Entity Framework Core

## Technologies Used

### Frontend Stack
- **Angular 19**: Modern standalone components with signals
- **Angular Material 19**: UI component library
- **NgRx Signals**: State management
- **Transloco**: Internationalization
- **TailwindCSS**: Utility-first styling
- **TypeScript**: Type-safe development

### Backend Stack
- **.NET 9**: Latest .NET platform
- **ASP.NET Core Web API**: RESTful API framework
- **Entity Framework Core**: Object-relational mapping
- **MediatR**: CQRS implementation
- **FluentValidation**: Input validation
- **Serilog**: Structured logging
- **SQL Server**: Database management system
- **Swagger/OpenAPI**: API documentation

### Development Tools
- **Docker**: Containerization
- **Azure**: Cloud deployment platform
- **GitHub Actions**: CI/CD pipeline

## Project Structure

```
taskin2.0/
├── back/                          # Backend (.NET 9)
│   └── src/Taskin.Api/           # Backend solution
│       ├── ElGuerre.Taskin.Api/          # Web API layer
│       │   ├── Controllers/              # API controllers
│       │   ├── Program.cs               # Application entry point
│       │   └── appsettings.json         # Configuration
│       ├── ElGuerre.Taskin.Application/  # Application layer (CQRS)
│       │   ├── Projects/                # Project commands/queries
│       │   ├── Tasks/                   # Task commands/queries
│       │   └── Pomodoros/              # Pomodoro commands/queries
│       ├── ElGuerre.Taskin.Domain/       # Domain entities
│       │   └── Entities/                # Core domain models
│       └── ElGuerre.Taskin.Infrastructure/ # Data access layer
│           ├── EntityFramework/         # EF Core context
│           │   ├── TaskinDbContext.cs
│           │   └── Configurations/
│           └── Migrations/             # Database migrations
├── ui/                           # Frontend (Angular 19)
│   └── src/taskin-angular/       # Angular application
│       ├── src/app/
│       │   ├── core/                   # Core services, guards
│       │   ├── features/               # Feature modules
│       │   │   ├── projects/          # Project management
│       │   │   ├── tasks/             # Task management
│       │   │   └── pomodoros/         # Pomodoro tracking
│       │   ├── layout/                # Application layout
│       │   └── shared/                # Shared components
│       ├── angular.json               # Angular CLI config
│       ├── package.json              # Dependencies
│       └── tsconfig.json             # TypeScript config
├── CLAUDE.md                     # Development guidelines
└── README.md                     # This file
```

## Getting Started

### Prerequisites

#### For Backend Development
- **.NET 9 SDK**: Download and install from [Microsoft .NET](https://dotnet.microsoft.com/download)
- **SQL Server**: Install SQL Server or use SQL Server Express
- **Visual Studio 2022** or **Visual Studio Code**

#### For Frontend Development
- **Node.js 18+**: Download from [nodejs.org](https://nodejs.org/)
- **Angular CLI**: Install globally with `npm install -g @angular/cli@19`
- **Visual Studio Code** with Angular Language Service extension

### Setup Instructions

1. **Clone the Repository**

   ```bash
   git clone https://github.com/your-username/Taskin2.0.git
   cd Taskin2.0
   ```

#### Backend Setup

2. **Navigate to Backend Directory**

   ```bash
   cd back/src/Taskin.Api
   ```

3. **Restore NuGet Packages**

   ```bash
   dotnet restore
   ```

4. **Configure Database Connection**

   Update the `appsettings.json` file in `ElGuerre.Taskin.Api` with your SQL Server connection string:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=your_server;Database=TaskinDB;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }
   ```

5. **Apply Database Migrations**

   ```bash
   dotnet ef migrations add InitialCreate --startup-project ElGuerre.Taskin.Api --project ElGuerre.Taskin.Infrastructure -o EntityFramework/Migrations
   dotnet ef database update --startup-project ElGuerre.Taskin.Api --project ElGuerre.Taskin.Infrastructure
   ```

#### Frontend Setup

6. **Navigate to Frontend Directory**

   ```bash
   cd ../../../ui/src/taskin-angular
   ```

7. **Install Dependencies**

   ```bash
   npm install
   ```

## Running the Application

### Option 1: Run Backend and Frontend Separately

#### Start the Backend (API)

1. Navigate to the backend directory:

   ```bash
   cd back/src/Taskin.Api
   ```

2. Run the API server:

   ```bash
   dotnet run --project ElGuerre.Taskin.Api
   ```

   The API will start on `https://localhost:5001` by default.

#### Start the Frontend (Angular)

1. Navigate to the frontend directory:

   ```bash
   cd ui/src/taskin-angular
   ```

2. Start the development server:

   ```bash
   npm start
   # or
   ng serve --port 4200
   ```

   The Angular app will be available at `http://localhost:4200`.

### Option 2: Docker (Coming Soon)

Docker Compose configuration for running both applications together will be available soon.

### Testing the Application

- **API Documentation**: Navigate to `https://localhost:5001/swagger` to view the Swagger UI
- **Frontend**: Open `http://localhost:4200` to access the Angular application

## Frontend (Angular)

### Development Commands

Run these commands from `/ui/src/taskin-angular/` directory:

```bash
# Start development server
npm start                    # Runs on http://localhost:4200

# Build for production
npm run build               # Outputs to dist/

# Run unit tests
npm test                    # Karma/Jasmine tests

# Build with watch mode
npm run watch              # Development build with file watching

# Generate new component
ng generate component features/feature-name/component-name --skip-tests --inline-style --change-detection OnPush
```

### Key Frontend Technologies

- **Angular 19**: Standalone components with signals
- **Angular Material**: Material Design components
- **NgRx Signals**: State management
- **Transloco**: i18n support
- **TailwindCSS**: Utility-first CSS

## Backend (ASP.NET Core)

### Development Commands

Run these commands from `/back/src/Taskin.Api/` directory:

```bash
# Build solution
dotnet build

# Run API server (https://localhost:5001)
dotnet run --project ElGuerre.Taskin.Api

# Run tests
dotnet test

# Database migrations
dotnet ef migrations add MigrationName --startup-project ElGuerre.Taskin.Api --project ElGuerre.Taskin.Infrastructure -o EntityFramework/Migrations
dotnet ef database update --startup-project ElGuerre.Taskin.Api --project ElGuerre.Taskin.Infrastructure
```

### Key Backend Patterns

- **CQRS**: Commands and Queries with MediatR
- **Clean Architecture**: Separated layers
- **Entity Framework Core**: Code-first approach
- **FluentValidation**: Request validation

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

To manage database schema changes, use Entity Framework Core migrations from the backend directory:

### Adding a Migration

```bash
# From /back/src/Taskin.Api/ directory
dotnet ef migrations add MigrationName --startup-project ElGuerre.Taskin.Api --project ElGuerre.Taskin.Infrastructure -o EntityFramework/Migrations
```

### Updating the Database

```bash
# From /back/src/Taskin.Api/ directory
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

For questions or support, please open an issue on the [GitHub repository](https://github.com/your-username/Taskin2.0/issues).

## Acknowledgments

### Frontend
- **Angular**: [https://angular.io/](https://angular.io/)
- **Angular Material**: [https://material.angular.io/](https://material.angular.io/)
- **NgRx**: [https://ngrx.io/](https://ngrx.io/)
- **Transloco**: [https://ngneat.github.io/transloco/](https://ngneat.github.io/transloco/)

### Backend
- **ASP.NET Core**: [https://docs.microsoft.com/aspnet/core](https://docs.microsoft.com/aspnet/core)
- **Entity Framework Core**: [https://docs.microsoft.com/ef/core](https://docs.microsoft.com/ef/core)
- **MediatR**: [https://github.com/jbogard/MediatR](https://github.com/jbogard/MediatR)
- **Clean Architecture**: [https://github.com/jasontaylordev/CleanArchitecture](https://github.com/jasontaylordev/CleanArchitecture)

### Development Tools
- **Docker**: [https://www.docker.com/](https://www.docker.com/)
- **Azure**: [https://azure.microsoft.com/](https://azure.microsoft.com/)

Thank you for using Task[in] 2.0! We hope this full-stack application helps you manage your productivity effectively using the Pomodoro Technique.
