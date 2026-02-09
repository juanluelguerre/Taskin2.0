# Backend CLAUDE.md — .NET 9 Clean Architecture

## Stack

.NET 9 | ASP.NET Core | EF Core | MediatR | FluentValidation | Serilog | OpenTelemetry

## Architecture Layers

```
Api (Controllers, Middleware) → Application (CQRS, DTOs, Validators) → Domain (Entities, Enums)
                                                                         ↑
                              Infrastructure (EF Core, Repos) ───────────┘
```

**Dependency rule**: Inner layers never reference outer layers. Domain has zero dependencies.

## Entity Pattern

```csharp
// Base entity hierarchy: Entity → TrackedEntity → Domain entities
public abstract class Entity
{
    public Guid Id { get; init; } = Guid.NewGuid();
}

public abstract class TrackedEntity : Entity
{
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// Domain entity
public sealed class Project : TrackedEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;
    public DateTime? DueDate { get; set; }
    public string? ImageUrl { get; set; }
    public string? BackgroundColor { get; set; }
    public ICollection<Task> Tasks { get; init; } = new List<Task>();
}
```

Rules:
- Use `required` for mandatory properties
- Use `init` for immutable properties (Id, CreatedAt, collections)
- Use nullable reference types (`string?`) for optional properties
- Use `sealed` on concrete entities
- Enums defined alongside their entity

## CQRS Pattern (Command)

Every command needs 3 files: Command + Handler + Validator.

### Command
```csharp
using MediatR;

namespace ElGuerre.Taskin.Application.Features.Commands;

public class CreateFeatureCommand : IRequest<Guid>
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
```

### Handler (primary constructor)
```csharp
using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Observability;
using ElGuerre.Taskin.Domain.Entities;
using ElGuerre.Taskin.Domain.SeedWork;
using MediatR;

namespace ElGuerre.Taskin.Application.Features.Commands;

public class CreateFeatureCommandHandler(
    ITaskinDbContext context,
    IUnitOfWork unitOfWork,
    TaskinMetrics metrics) : IRequestHandler<CreateFeatureCommand, Guid>
{
    public async Task<Guid> Handle(
        CreateFeatureCommand request,
        CancellationToken cancellationToken)
    {
        var entity = new Feature
        {
            Name = request.Name,
            Description = request.Description,
        };

        context.Features.Add(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        metrics.RecordFeatureCreated();

        return entity.Id;
    }
}
```

### Validator (FluentValidation)
```csharp
using FluentValidation;

namespace ElGuerre.Taskin.Application.Features.Commands;

public class CreateFeatureCommandValidator : AbstractValidator<CreateFeatureCommand>
{
    public CreateFeatureCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}
```

## CQRS Pattern (Query)

```csharp
// Query
public class GetFeaturesQuery : IRequest<CollectionResponse<FeatureListDto>>
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Search { get; set; }
}

// Handler
public class GetFeaturesQueryHandler(ITaskinDbContext context)
    : IRequestHandler<GetFeaturesQuery, CollectionResponse<FeatureListDto>>
{
    public async Task<CollectionResponse<FeatureListDto>> Handle(
        GetFeaturesQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.Features.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(f => f.Name.Contains(request.Search));

        var total = await query.CountAsync(cancellationToken);
        var data = await query
            .Skip((request.Page - 1) * request.Size)
            .Take(request.Size)
            .Select(f => new FeatureListDto { /* map */ })
            .ToListAsync(cancellationToken);

        return new CollectionResponse<FeatureListDto>(data, total, request.Page, request.Size);
    }
}
```

## Controller Pattern

```csharp
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ElGuerre.Taskin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeaturesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CollectionResponse<FeatureListDto>>> GetFeatures(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? search = null)
    {
        var result = await mediator.Send(new GetFeaturesQuery { Page = page, Size = size, Search = search });
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FeatureDetailsDto>> GetFeature(Guid id)
    {
        var result = await mediator.Send(new GetFeatureByIdQuery { Id = id });
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ActionResponse>> CreateFeature([FromBody] CreateFeatureCommand command)
    {
        var id = await mediator.Send(command);
        var response = new ActionResponse(id, "Feature created successfully");
        return CreatedAtAction(nameof(GetFeature), new { id }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ActionResponse>> UpdateFeature(Guid id, [FromBody] UpdateFeatureCommand command)
    {
        command.Id = id;
        await mediator.Send(command);
        return Ok(new ActionResponse(id, "Feature updated successfully"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ActionResponse>> DeleteFeature(Guid id)
    {
        await mediator.Send(new DeleteFeatureCommand { Id = id });
        return Ok(new ActionResponse(id, "Feature deleted successfully"));
    }
}
```

Rules:
- Primary constructor with `IMediator`
- `[ApiController]` + `[Route("api/[controller]")]`
- POST returns `CreatedAtAction` (201)
- PUT/DELETE returns `Ok` (200)
- Use `{id:guid}` route constraint

## EF Core Configuration

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElGuerre.Taskin.Infrastructure.EntityFramework.EntityConfigurations;

public class FeatureEntityTypeConfiguration : IEntityTypeConfiguration<Feature>
{
    public void Configure(EntityTypeBuilder<Feature> builder)
    {
        builder.ToTable("Features");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Name).IsRequired().HasMaxLength(200);
        builder.Property(f => f.Description).HasMaxLength(1000);

        builder.HasMany(f => f.Children)
            .WithOne(c => c.Parent)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

## DTO Patterns

```csharp
// Response wrappers
public record CollectionResponse<T>(IReadOnlyList<T> Data, int Total, int Page, int Size);
public record ActionResponse(Guid Id, string Message, bool Success = true);

// List DTO (for collections)
public class FeatureListDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
}

// Details DTO (for single entity with relations)
public class FeatureDetailsDto : FeatureListDto
{
    public List<ChildDto> Children { get; set; } = [];
}
```

## Metrics (Observability)

```csharp
using System.Diagnostics.Metrics;

namespace ElGuerre.Taskin.Application.Observability;

public class TaskinMetrics
{
    private readonly Counter<long> _projectsCreated;
    private readonly UpDownCounter<long> _activeProjects;

    public TaskinMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(TelemetryConstants.MeterName);
        _projectsCreated = meter.CreateCounter<long>("taskin.projects.created");
        _activeProjects = meter.CreateUpDownCounter<long>("taskin.projects.active");
    }

    public void RecordProjectCreated() => _projectsCreated.Add(1);
    public void IncrementActiveProjects() => _activeProjects.Add(1);
}
```

## Migration Commands

```bash
# From back/src/ directory
dotnet ef migrations add MigrationName \
  --startup-project ElGuerre.Taskin.Api \
  --project ElGuerre.Taskin.Infrastructure \
  -o EntityFramework/Migrations

dotnet ef database update \
  --startup-project ElGuerre.Taskin.Api \
  --project ElGuerre.Taskin.Infrastructure
```

## File Structure for New Features

```
Application/
└── FeatureName/
    ├── Commands/
    │   ├── CreateFeatureCommand.cs
    │   ├── CreateFeatureCommandHandler.cs
    │   ├── CreateFeatureCommandValidator.cs
    │   ├── UpdateFeatureCommand.cs
    │   ├── UpdateFeatureCommandHandler.cs
    │   ├── UpdateFeatureCommandValidator.cs
    │   ├── DeleteFeatureCommand.cs
    │   └── DeleteFeatureCommandHandler.cs
    ├── Queries/
    │   ├── GetFeaturesQuery.cs
    │   ├── GetFeaturesQueryHandler.cs
    │   ├── GetFeatureByIdQuery.cs
    │   └── GetFeatureByIdQueryHandler.cs
    └── DTOs/
        ├── FeatureListDto.cs
        └── FeatureDetailsDto.cs
```

## Quality Checklist

- [ ] Every command has a corresponding validator
- [ ] Handlers use primary constructor DI
- [ ] Controllers use primary constructor with `IMediator`
- [ ] `required` on mandatory properties
- [ ] Nullable reference types (`string?`) for optional
- [ ] Metrics recorded on important operations
- [ ] POST returns `CreatedAtAction` (HTTP 201)
- [ ] `{id:guid}` route constraints on endpoints
- [ ] EF config uses `IEntityTypeConfiguration<T>`
- [ ] No business logic in controllers — delegate to handlers
