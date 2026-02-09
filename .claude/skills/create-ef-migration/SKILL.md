---
name: create-ef-migration
description: Create and apply an Entity Framework Core migration with correct project paths
user_invocable: true
---

# Create EF Migration Skill

Create and optionally apply an Entity Framework Core migration.

## Arguments

The user provides: `<MigrationName>` (e.g., `AddTaskPriority`)

## Steps

1. **Verify pending changes** exist in entity configurations:
   ```bash
   cd back/src && dotnet build
   ```

2. **Create the migration**:
   ```bash
   cd back/src && dotnet ef migrations add {MigrationName} \
     --startup-project ElGuerre.Taskin.Api \
     --project ElGuerre.Taskin.Infrastructure \
     -o EntityFramework/Migrations
   ```

3. **Review the generated migration** file in:
   `back/src/ElGuerre.Taskin.Infrastructure/EntityFramework/Migrations/`

   Verify:
   - Table names are correct
   - Column types and constraints are correct
   - Foreign keys and indexes are properly configured
   - No unintended changes

4. **Ask user** if they want to apply the migration to the database:
   ```bash
   cd back/src && dotnet ef database update \
     --startup-project ElGuerre.Taskin.Api \
     --project ElGuerre.Taskin.Infrastructure
   ```

## Important Paths

- **Startup project**: `ElGuerre.Taskin.Api`
- **Migrations project**: `ElGuerre.Taskin.Infrastructure`
- **Output directory**: `EntityFramework/Migrations`
- **Working directory**: `back/src/`

## Checklist

- [ ] Backend builds successfully before migration
- [ ] Migration name is descriptive (PascalCase, e.g., `AddTaskPriority`)
- [ ] Generated migration reviewed for correctness
- [ ] No data loss warnings
- [ ] User confirmed before applying to database
