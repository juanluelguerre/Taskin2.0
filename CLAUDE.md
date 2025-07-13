# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Task[in] 2.0 Backend is an ASP.NET Core Web API following Clean Architecture principles with CQRS pattern using MediatR. It's a productivity application for managing projects, tasks, and Pomodoro time tracking sessions.

## Common Commands

**Working Directory**: `/back/src/Taskin.Api/`

- **Build**: `dotnet build`
- **Run API**: `dotnet run --project ElGuerre.Taskin.Api`
- **Restore packages**: `dotnet restore`
- **Clean**: `dotnet clean`
- **Database migrations**:
  - Add: `dotnet ef migrations add <MigrationName> --startup-project .\ElGuerre.Taskin.Api --project .\ElGuerre.Taskin.Infrastructure -o .\EntityFramework\Migrations`
  - Update: `dotnet ef database update --startup-project .\ElGuerre.Taskin.Api\ElGuerre.Taskin.Api.csproj --project .\ElGuerre.Taskin.Infrastructure\`
- **API Documentation**: Access Swagger at `https://localhost:5001/swagger` when running
- **Health Check**: `https://localhost:5001/health`

## Clean Architecture Structure

### **1. ElGuerre.Taskin.Api (Presentation Layer)**
**Purpose**: Controllers, API configuration, middleware registration

**Key Files:**
- `Program.cs`: Application entry point with DI setup and middleware pipeline
- `Controllers/`: API controllers (ProjectsController, TasksController, PomodorosController)
- `Extensions/BuilderExtensions.cs`: Custom middleware extensions
- `Models/`: API-specific DTOs (currently Pomodoro.cs and Task.cs)

**Dependencies**: Application, Infrastructure layers

### **2. ElGuerre.Taskin.Application (Application Layer)**
**Purpose**: Business logic, CQRS commands/queries, handlers, behaviors

**Structure:**
```
/Behaviors/
  - LoggingBehavior.cs         # MediatR pipeline logging
  - ValidationBehavior.cs      # FluentValidation integration
/Data/
  - ITaskinDbContext.cs        # DbContext interface
/Errors/
  - Error.cs, Result.cs        # Error handling types
/Exceptions/
  - [Various]Exception.cs      # Custom business exceptions
/[Feature]/Commands/           # CQRS Commands
/[Feature]/Queries/            # CQRS Queries
```

**CQRS Operations:**
- **Projects**: ✅ Create, ✅ GetProjects (❌ Missing: GetById, Update, Delete)
- **Tasks**: ✅ Complete CRUD (Create, GetById, GetByProjectId, Update, Delete)
- **Pomodoros**: ✅ Complete CRUD (Create, GetById, GetByTaskId, Update, Delete)

**Dependencies**: Domain layer only

### **3. ElGuerre.Taskin.Domain (Domain Layer)**
**Purpose**: Domain entities, business rules, interfaces

**Entities Hierarchy:**
```
Entity (abstract base)
└── TrackedEntity (adds CreatedAt, UpdatedAt)
    ├── Project
    ├── Task
    └── Pomodoro
```

**Entity Relationships:**
```
Project (1) ──────→ (∞) Task (1) ──────→ (∞) Pomodoro
        Name                Description            StartTime
        ImageUrl            Status (enum)          EndTime
        BackgroundColor     Deadline               Notes
```

**SeedWork Interfaces:**
- `IUnitOfWork`, `ITransaction`: Transaction management patterns

**Dependencies**: None (pure domain)

### **4. ElGuerre.Taskin.Infrastructure (Infrastructure Layer)**
**Purpose**: Data access, Entity Framework, external dependencies, middleware

**Structure:**
```
/EntityFramework/
  - TaskinDbContext.cs                    # Main DbContext
  /EntityConfigurations/                  # EF Core configurations
  /Migrations/                           # Database migrations
/Middleware/
  - ErrorHandlingMiddleware.cs           # Global exception handling
  - InfrastructureExtensions.cs          # DI registration
```

**Dependencies**: Application, Domain layers

## API Endpoints

### Projects API (`/api/Projects`)
- `GET /api/Projects` ✅ - List projects (with pagination support)
- `POST /api/Projects` ✅ - Create project
- `GET /api/Projects/{id}` ❌ - **Missing**
- `PUT /api/Projects/{id}` ❌ - **Missing**  
- `DELETE /api/Projects/{id}` ❌ - **Missing**

### Tasks API (`/api/Tasks`)
- `GET /api/Tasks/{id}` ✅ - Get task by ID
- `GET /api/Tasks?projectId={id}` ✅ - Get tasks by project
- `POST /api/Tasks` ✅ - Create task
- `PUT /api/Tasks/{id}` ✅ - Update task
- `DELETE /api/Tasks/{id}` ✅ - Delete task

### Pomodoros API (`/api/Pomodoros`)
- `GET /api/Pomodoros/{id}` ✅ - Get pomodoro by ID
- `GET /api/Pomodoros?taskId={id}` ✅ - Get pomodoros by task
- `POST /api/Pomodoros` ✅ - Create pomodoro
- `PUT /api/Pomodoros/{id}` ✅ - Update pomodoro
- `DELETE /api/Pomodoros/{id}` ✅ - Delete pomodoro

## CQRS Implementation Patterns

### Command Pattern
```csharp
// 1. Command class
public class CreateTaskCommand : IRequest<Guid>
{
    public string Description { get; set; }
    public Guid ProjectId { get; set; }
    // ...
}

// 2. Handler class  
public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Guid>
{
    private readonly ITaskinDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<Guid> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        // Business logic here
        return entity.Id;
    }
}

// 3. Validator class
public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Description).NotEmpty();
        // ...
    }
}
```

### Query Pattern
```csharp
// Query + Handler in same pattern
public class GetTaskByIdQuery : IRequest<Domain.Entities.Task>
public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, Domain.Entities.Task>
```

## Error Handling Architecture

**Exception Hierarchy:**
```
TaskinExceptionBase (abstract)
├── BusinessException
├── EntityNotFoundException<T>
├── EntityAlreadyExistsException<T>
├── ForbiddenEntityException<T>
├── KeyAlreadyExistsException<T>
└── UnauthorizedException
```

**HTTP Status Mapping:**
- `EntityNotFoundException` → 404 Not Found
- `BusinessException` → 400 Bad Request
- `UnauthorizedException` → 401 Unauthorized
- `ForbiddenEntityException` → 403 Forbidden
- `EntityAlreadyExistsException` → 409 Conflict
- `KeyAlreadyExistsException` → 409 Conflict

## Database Configuration

**Provider**: SQL Server with Entity Framework Core 8.0.10
**Strategy**: Code-First with migrations
**Connection**: `appsettings.json` → `ConnectionStrings:DefaultConnection`

**Key Configurations:**
- Cascade delete behavior properly configured
- String length constraints defined
- Required fields marked appropriately
- Indexes on foreign keys

## Dependencies & Versions

**Framework**: .NET 9.0

**Key Packages:**
- **MediatR 12.4.1**: CQRS implementation
- **Entity Framework Core 8.0.10**: Data access
- **FluentValidation 11.10.0**: Command validation
- **Serilog.AspNetCore 8.0.3**: Structured logging
- **Swashbuckle.AspNetCore 6.8.1**: OpenAPI documentation

## Critical Issues & Missing Components

### 🔴 **Critical Issues (Application Won't Run)**

1. **IUnitOfWork Implementation Missing**
   - Interface exists in Domain but no implementation
   - Not registered in DI container
   - Handlers depend on this for transaction management

2. **MediatR Registration Bug**
   - Currently scans Infrastructure assembly instead of Application
   - Fix: Change to `typeof(CreateProjectCommand).Assembly` in InfrastructureExtensions.cs

3. **DI Registration Issues**
   - `ITaskinDbContext` interface not registered
   - FluentValidation validators not registered

### 🟡 **Incomplete Features**

1. **Missing CQRS Operations**
   - `GetProjectByIdQuery` and handler
   - `UpdateProjectCommand` and handler
   - `DeleteProjectCommand` and handler

2. **Missing Validators**
   - Task command validators
   - Pomodoro command validators

3. **Entity Issues**
   - `Entity.Id` property is get-only, problematic for EF Core
   - TrackedEntity timestamps not automatically managed

### 🔧 **Bug Fixes Needed**

1. **CreateTaskCommandHandler Line 18**: Throws wrong exception type (`EntityNotFoundException<Pomodoro>` instead of `EntityNotFoundException<Project>`)

## Development Guidelines

1. **CQRS Pattern**: Always create separate Command/Query with dedicated Handler
2. **Validation**: Add FluentValidation validator for each command
3. **Error Handling**: Use appropriate custom exceptions, never throw generic exceptions
4. **Entity Changes**: All domain entity changes must use EF migrations
5. **Logging**: Use structured logging with Serilog, leverage LoggingBehavior
6. **Controllers**: Keep thin, only orchestrate MediatR requests
7. **Unit of Work**: Use IUnitOfWork for transaction boundaries
8. **Async/Await**: All database operations must be async