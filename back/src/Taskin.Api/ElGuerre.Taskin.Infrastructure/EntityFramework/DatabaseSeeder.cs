using ElGuerre.Taskin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;
using TaskStatus = ElGuerre.Taskin.Domain.Entities.TaskStatus;

namespace ElGuerre.Taskin.Infrastructure.EntityFramework;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TaskinDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TaskinDbContext>>();

        try
        {
            await context.Database.EnsureCreatedAsync();

            if (!await context.Projects.AnyAsync())
            {
                await SeedProjectsAsync(context);
                logger.LogInformation("Database seeded successfully with initial projects data");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private static async Task SeedProjectsAsync(TaskinDbContext context)
    {
        Project project1 = new Project
        {
            Name = "E-commerce Platform",
            Description =
                "Building a modern e-commerce solution with React and Node.js. This includes payment integration, inventory management, and customer portal.",
            Status = ProjectStatus.Active,
            DueDate = DateTime.UtcNow.AddDays(45),
            ImageUrl =
                "https://images.unsplash.com/photo-1556742049-0cfed4f6a45d?w=800&h=400&fit=crop",
            BackgroundColor = "#3B82F6"
        };

        Project project2 = new Project
        {
            Name = "Mobile App Redesign",
            Description =
                "Complete UI/UX overhaul of our mobile application with modern design principles and improved user experience.",
            Status = ProjectStatus.Active,
            DueDate = DateTime.UtcNow.AddDays(30),
            ImageUrl =
                "https://images.unsplash.com/photo-1512941937669-90a1b58e7e9c?w=800&h=400&fit=crop",
            BackgroundColor = "#10B981"
        };

        Project project3 = new Project
        {
            Name = "API Documentation",
            Description =
                "Comprehensive API documentation for developers including endpoints, authentication, and examples.",
            Status = ProjectStatus.Completed,
            DueDate = DateTime.UtcNow.AddDays(-5),
            ImageUrl =
                "https://images.unsplash.com/photo-1555949963-aa79dcee981c?w=800&h=400&fit=crop",
            BackgroundColor = "#8B5CF6"
        };

        Project project4 = new Project
        {
            Name = "Marketing Website",
            Description =
                "New landing page with improved SEO, performance optimization, and modern design to increase conversion rates.",
            Status = ProjectStatus.OnHold,
            DueDate = DateTime.UtcNow.AddDays(60),
            ImageUrl =
                "https://images.unsplash.com/photo-1460925895917-afdab827c52f?w=800&h=400&fit=crop",
            BackgroundColor = "#F59E0B"
        };

        Project project5 = new Project
        {
            Name = "Data Analytics Dashboard",
            Description =
                "Real-time analytics dashboard for business intelligence with interactive charts, KPI tracking, and automated reporting.",
            Status = ProjectStatus.Active,
            DueDate = DateTime.UtcNow.AddDays(75),
            ImageUrl =
                "https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=800&h=400&fit=crop",
            BackgroundColor = "#EF4444"
        };

        Project project6 = new Project
        {
            Name = "Security Audit",
            Description =
                "Comprehensive security review and vulnerability assessment of the entire application infrastructure.",
            Status = ProjectStatus.Active,
            DueDate = DateTime.UtcNow.AddDays(20),
            ImageUrl =
                "https://images.unsplash.com/photo-1563986768609-322da13575f3?w=800&h=400&fit=crop",
            BackgroundColor = "#6366F1"
        };

        List<Project> projects = [project1, project2, project3, project4, project5, project6];

        // Set creation timestamps manually since we're seeding
        foreach (Project project in projects)
        {
            project.SetCreationInfo();
            project.SetModificationInfo();
        }

        await context.Projects.AddRangeAsync(projects);
        await context.SaveChangesAsync();

        // Add some tasks for the projects
        await SeedTasksAsync(context, projects);
    }

    private static async Task SeedTasksAsync(TaskinDbContext context, List<Project> projects)
    {
        List<Domain.Entities.Task> tasks = [];

        // Tasks for E-commerce Platform
        Project ecommerceProject = projects[0];
        tasks.AddRange(
        [
            new Domain.Entities.Task
            {
                Description = "Setup project structure and initialize React app",
                ProjectId = ecommerceProject.Id,
                Project = ecommerceProject,
                Status = TaskStatus.Done
            },
            new Domain.Entities.Task
            {
                Description = "Design responsive product catalog pages",
                ProjectId = ecommerceProject.Id,
                Project = ecommerceProject,
                Status = TaskStatus.Doing
            },
            new Domain.Entities.Task
            {
                Description = "Integrate Stripe payment gateway",
                ProjectId = ecommerceProject.Id,
                Project = ecommerceProject,
                Status = TaskStatus.Todo
            },
            new Domain.Entities.Task
            {
                Description = "Implement user authentication with JWT",
                ProjectId = ecommerceProject.Id,
                Project = ecommerceProject,
                Status = TaskStatus.Todo
            }
        ]);

        // Tasks for Mobile App Redesign
        Project mobileProject = projects[1];
        tasks.AddRange(
        [
            new Domain.Entities.Task
            {
                Description = "Conduct user research and interviews",
                ProjectId = mobileProject.Id,
                Project = mobileProject,
                Status = TaskStatus.Done
            },
            new Domain.Entities.Task
            {
                Description = "Create wireframes and user flows",
                ProjectId = mobileProject.Id,
                Project = mobileProject,
                Status = TaskStatus.Done
            },
            new Domain.Entities.Task
            {
                Description = "Establish design system and components",
                ProjectId = mobileProject.Id,
                Project = mobileProject,
                Status = TaskStatus.Doing
            },
            new Domain.Entities.Task
            {
                Description = "Build interactive prototype",
                ProjectId = mobileProject.Id,
                Project = mobileProject,
                Status = TaskStatus.Todo
            }
        ]);

        // Tasks for API Documentation
        Project apiProject = projects[2];
        tasks.AddRange(
        [
            new Domain.Entities.Task
            {
                Description = "Document REST API endpoints",
                ProjectId = apiProject.Id,
                Project = apiProject,
                Status = TaskStatus.Done
            },
            new Domain.Entities.Task
            {
                Description = "Create authentication flow guide",
                ProjectId = apiProject.Id,
                Project = apiProject,
                Status = TaskStatus.Done
            },
            new Domain.Entities.Task
            {
                Description = "Add practical code examples",
                ProjectId = apiProject.Id,
                Project = apiProject,
                Status = TaskStatus.Done
            }
        ]);

        // Set timestamps for tasks
        foreach (Domain.Entities.Task task in tasks)
        {
            task.SetCreationInfo();
            task.SetModificationInfo();
        }

        await context.Tasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
    }
}