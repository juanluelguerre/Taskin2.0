---
name: create-feature
description: Full-stack feature scaffolding - backend entity through frontend components with translations
user_invocable: true
---

# Create Feature Skill

Scaffold a complete full-stack feature from domain entity to UI components.

## Arguments

The user provides: `<FeatureName>` and a description of the feature's properties/behavior.

## Steps

### Phase 1: Backend

1. **Domain Entity** — `back/src/ElGuerre.Taskin.Domain/Entities/{Feature}.cs`
   - Extend `TrackedEntity`
   - Use `required` for mandatory properties
   - Use nullable refs for optional
   - Define related enum if needed

2. **EF Configuration** — `back/src/ElGuerre.Taskin.Infrastructure/EntityFramework/EntityConfigurations/{Feature}EntityTypeConfiguration.cs`
   - Implement `IEntityTypeConfiguration<Feature>`
   - Configure table name, key, property constraints, relationships

3. **Register DbSet** — Add `DbSet<Feature>` to `ITaskinDbContext` and `TaskinDbContext`

4. **DTOs** — `back/src/ElGuerre.Taskin.Application/{Features}/DTOs/`
   - `{Feature}ListDto.cs`
   - `{Feature}DetailsDto.cs`

5. **CQRS Commands** — `back/src/ElGuerre.Taskin.Application/{Features}/Commands/`
   - Create: Command + Handler + Validator
   - Update: Command + Handler + Validator
   - Delete: Command + Handler

6. **CQRS Queries** — `back/src/ElGuerre.Taskin.Application/{Features}/Queries/`
   - GetAll: Query + Handler
   - GetById: Query + Handler

7. **Controller** — `back/src/ElGuerre.Taskin.Api/Controllers/{Features}Controller.cs`
   - Primary constructor with `IMediator`
   - CRUD endpoints

8. **Migration**:
   ```bash
   cd back/src && dotnet ef migrations add Add{Feature} \
     --startup-project ElGuerre.Taskin.Api \
     --project ElGuerre.Taskin.Infrastructure \
     -o EntityFramework/Migrations
   ```

9. **Verify backend build**:
   ```bash
   cd back/src && dotnet build
   ```

### Phase 2: Frontend

10. **Service** — `ui/src/src/app/features/{features}/services/{feature}.service.ts`
    - HTTP methods for all CRUD operations
    - DTO interfaces matching backend
    - `CollectionResponse<T>` and `ActionResponse` types

11. **Store** — `ui/src/src/app/features/{features}/stores/{feature}.store.ts`
    - NgRx Signal Store pattern
    - `switchMap` for queries, `exhaustMap` for mutations
    - Error handling with `NotificationService`

12. **Components** — Generate using Angular CLI:
    ```bash
    cd ui/src && npx ng g c features/{features}/pages/{features} --skip-tests --inline-style --change-detection OnPush --view-encapsulation None
    cd ui/src && npx ng g c features/{features}/pages/{feature}-details --skip-tests --inline-style --change-detection OnPush --view-encapsulation None
    cd ui/src && npx ng g c features/{features}/pages/{feature}-new --skip-tests --inline-style --change-detection OnPush --view-encapsulation None
    ```

13. **Routes** — `ui/src/src/app/features/{features}/{features}.routes.ts`

14. **Translations** — Add keys to both `en.json` and `es.json`

15. **Navigation** — Add menu item to sidebar

16. **Verify frontend build**:
    ```bash
    cd ui/src && npm run build
    ```

## Output Summary

After completion, list all created files grouped by layer:
- Domain (entity, enum)
- Infrastructure (EF config, migration)
- Application (DTOs, commands, queries)
- API (controller)
- Frontend (service, store, components, routes, translations)
