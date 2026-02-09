---
name: create-cqrs-handler
description: Create a complete CQRS command or query with handler and validator
user_invocable: true
---

# Create CQRS Handler Skill

Create a complete CQRS command/query set following Taskin 2.0 backend conventions.

## Arguments

The user provides: `<Feature> <Operation>` (e.g., `Project Create`, `Task GetById`)

## Steps

1. **Determine type**: Command (Create/Update/Delete) or Query (Get/GetById/GetStats)

2. **Create the files** in `back/src/ElGuerre.Taskin.Application/{Feature}/{Commands|Queries}/`:

### For Commands (Create/Update/Delete):

**Command class** — `{Operation}{Feature}Command.cs`:
```csharp
using MediatR;
namespace ElGuerre.Taskin.Application.{Features}.Commands;

public class {Operation}{Feature}Command : IRequest<Guid>
{
    public required string Name { get; set; }
    // Properties based on entity
}
```

**Handler** — `{Operation}{Feature}CommandHandler.cs`:
```csharp
using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Observability;
using ElGuerre.Taskin.Domain.Entities;
using ElGuerre.Taskin.Domain.SeedWork;
using MediatR;

namespace ElGuerre.Taskin.Application.{Features}.Commands;

public class {Operation}{Feature}CommandHandler(
    ITaskinDbContext context,
    IUnitOfWork unitOfWork,
    TaskinMetrics metrics) : IRequestHandler<{Operation}{Feature}Command, Guid>
{
    public async Task<Guid> Handle({Operation}{Feature}Command request, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

**Validator** — `{Operation}{Feature}CommandValidator.cs`:
```csharp
using FluentValidation;
namespace ElGuerre.Taskin.Application.{Features}.Commands;

public class {Operation}{Feature}CommandValidator : AbstractValidator<{Operation}{Feature}Command>
{
    public {Operation}{Feature}CommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
```

### For Queries:

**Query class** + **Handler** (similar pattern with `IRequest<CollectionResponse<T>>` or `IRequest<DetailsDto>`).

3. **Create DTOs** if needed in `back/src/ElGuerre.Taskin.Application/{Feature}/DTOs/`

4. **Verify build**:
```bash
cd back/src && dotnet build 2>&1 | tail -5
```

## Conventions

- Handlers use **primary constructor** DI
- Commands return `Guid` (the created/affected entity ID)
- Queries return `CollectionResponse<T>` or specific DTO
- Every command MUST have a validator
- Record metrics on important operations
- Use `ITaskinDbContext` and `IUnitOfWork` from Application layer
