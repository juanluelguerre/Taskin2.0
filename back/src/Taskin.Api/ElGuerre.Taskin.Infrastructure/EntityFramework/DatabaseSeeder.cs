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

        // Add comprehensive tasks data for the projects
        await SeedTasksAsync(context, projects);
        
        // Add some pomodoros for completed tasks
        await SeedPomodorosAsync(context);
    }

    private static async Task SeedTasksAsync(TaskinDbContext context, List<Project> projects)
    {
        List<Domain.Entities.Task> tasks = [];

        // Tasks for E-commerce Platform (project[0])
        Project ecommerceProject = projects[0];
        tasks.AddRange(
        [
            new Domain.Entities.Task
            {
                Description = "Setup project structure and initialize React app with TypeScript, ESLint, and modern tooling configuration",
                ProjectId = ecommerceProject.Id,
                Project = ecommerceProject,
                Status = TaskStatus.Done,
                Deadline = DateTime.UtcNow.AddDays(-10)
            },
            new Domain.Entities.Task
            {
                Description = "Design responsive product catalog pages with filtering, search, and pagination functionality",
                ProjectId = ecommerceProject.Id,
                Project = ecommerceProject,
                Status = TaskStatus.Doing,
                Deadline = DateTime.UtcNow.AddDays(5)
            },
            new Domain.Entities.Task
            {
                Description = "Integrate Stripe payment gateway with secure checkout flow and webhook handling",
                ProjectId = ecommerceProject.Id,
                Project = ecommerceProject,
                Status = TaskStatus.Todo,
                Deadline = DateTime.UtcNow.AddDays(15)
            },
            new Domain.Entities.Task
            {
                Description = "Implement user authentication with JWT tokens and refresh mechanism",
                ProjectId = ecommerceProject.Id,
                Project = ecommerceProject,
                Status = TaskStatus.Todo,
                Deadline = DateTime.UtcNow.AddDays(8)
            },
            new Domain.Entities.Task
            {
                Description = "Create shopping cart functionality with persistent storage and real-time updates",
                ProjectId = ecommerceProject.Id,
                Project = ecommerceProject,
                Status = TaskStatus.Todo,
                Deadline = DateTime.UtcNow.AddDays(12)
            }
        ]);

        // Tasks for Mobile App Redesign (project[1])
        Project mobileProject = projects[1];
        tasks.AddRange(
        [
            new Domain.Entities.Task
            {
                Description = "Conduct comprehensive user research and stakeholder interviews to understand pain points",
                ProjectId = mobileProject.Id,
                Project = mobileProject,
                Status = TaskStatus.Done,
                Deadline = DateTime.UtcNow.AddDays(-20)
            },
            new Domain.Entities.Task
            {
                Description = "Create detailed wireframes and user journey flows for all major app sections",
                ProjectId = mobileProject.Id,
                Project = mobileProject,
                Status = TaskStatus.Done,
                Deadline = DateTime.UtcNow.AddDays(-15)
            },
            new Domain.Entities.Task
            {
                Description = "Establish comprehensive design system with components, colors, and typography guidelines",
                ProjectId = mobileProject.Id,
                Project = mobileProject,
                Status = TaskStatus.Doing,
                Deadline = DateTime.UtcNow.AddDays(3)
            },
            new Domain.Entities.Task
            {
                Description = "Build interactive prototype with animations and micro-interactions for user testing",
                ProjectId = mobileProject.Id,
                Project = mobileProject,
                Status = TaskStatus.Todo,
                Deadline = DateTime.UtcNow.AddDays(10)
            },
            new Domain.Entities.Task
            {
                Description = "Conduct usability testing sessions and iterate based on user feedback",
                ProjectId = mobileProject.Id,
                Project = mobileProject,
                Status = TaskStatus.Todo,
                Deadline = DateTime.UtcNow.AddDays(18)
            }
        ]);

        // Tasks for API Documentation (project[2])
        Project apiProject = projects[2];
        tasks.AddRange(
        [
            new Domain.Entities.Task
            {
                Description = "Document all REST API endpoints with request/response examples and error codes",
                ProjectId = apiProject.Id,
                Project = apiProject,
                Status = TaskStatus.Done,
                Deadline = DateTime.UtcNow.AddDays(-8)
            },
            new Domain.Entities.Task
            {
                Description = "Create comprehensive authentication flow guide with JWT implementation details",
                ProjectId = apiProject.Id,
                Project = apiProject,
                Status = TaskStatus.Done,
                Deadline = DateTime.UtcNow.AddDays(-6)
            },
            new Domain.Entities.Task
            {
                Description = "Add practical code examples in multiple programming languages (Python, JavaScript, cURL)",
                ProjectId = apiProject.Id,
                Project = apiProject,
                Status = TaskStatus.Done,
                Deadline = DateTime.UtcNow.AddDays(-3)
            }
        ]);

        // Tasks for Marketing Website (project[3])
        Project marketingProject = projects[3];
        tasks.AddRange(
        [
            new Domain.Entities.Task
            {
                Description = "Design conversion-optimized landing page with clear value proposition and CTAs",
                ProjectId = marketingProject.Id,
                Project = marketingProject,
                Status = TaskStatus.Todo,
                Deadline = DateTime.UtcNow.AddDays(25)
            },
            new Domain.Entities.Task
            {
                Description = "Implement SEO best practices and schema markup for better search visibility",
                ProjectId = marketingProject.Id,
                Project = marketingProject,
                Status = TaskStatus.Todo,
                Deadline = DateTime.UtcNow.AddDays(30)
            }
        ]);

        // Tasks for Data Analytics Dashboard (project[4])
        Project analyticsProject = projects[4];
        tasks.AddRange(
        [
            new Domain.Entities.Task
            {
                Description = "Design interactive charts and KPI widgets with real-time data visualization",
                ProjectId = analyticsProject.Id,
                Project = analyticsProject,
                Status = TaskStatus.Doing,
                Deadline = DateTime.UtcNow.AddDays(20)
            },
            new Domain.Entities.Task
            {
                Description = "Implement automated report generation with PDF export and email scheduling",
                ProjectId = analyticsProject.Id,
                Project = analyticsProject,
                Status = TaskStatus.Todo,
                Deadline = DateTime.UtcNow.AddDays(35)
            },
            new Domain.Entities.Task
            {
                Description = "Set up data pipelines and ETL processes for business intelligence metrics",
                ProjectId = analyticsProject.Id,
                Project = analyticsProject,
                Status = TaskStatus.Todo,
                Deadline = DateTime.UtcNow.AddDays(40)
            }
        ]);

        // Tasks for Security Audit (project[5])
        Project securityProject = projects[5];
        tasks.AddRange(
        [
            new Domain.Entities.Task
            {
                Description = "Conduct penetration testing on web application and API endpoints",
                ProjectId = securityProject.Id,
                Project = securityProject,
                Status = TaskStatus.Doing,
                Deadline = DateTime.UtcNow.AddDays(7)
            },
            new Domain.Entities.Task
            {
                Description = "Review code for security vulnerabilities and implement OWASP recommendations",
                ProjectId = securityProject.Id,
                Project = securityProject,
                Status = TaskStatus.Todo,
                Deadline = DateTime.UtcNow.AddDays(14)
            },
            new Domain.Entities.Task
            {
                Description = "Set up automated security scanning and monitoring systems",
                ProjectId = securityProject.Id,
                Project = securityProject,
                Status = TaskStatus.Todo,
                Deadline = DateTime.UtcNow.AddDays(18)
            }
        ]);

        // Set creation and modification timestamps
        foreach (Domain.Entities.Task task in tasks)
        {
            task.SetCreationInfo();
            task.SetModificationInfo();
        }

        await context.Tasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
    }
    
    private static async Task SeedPomodorosAsync(TaskinDbContext context)
    {
        // Add pomodoros for completed and in-progress tasks
        var doneTasks = await context.Tasks
            .Where(t => t.Status == TaskStatus.Done)
            .ToListAsync();
            
        var doingTasks = await context.Tasks
            .Where(t => t.Status == TaskStatus.Doing)
            .Take(3) // Only some of the doing tasks
            .ToListAsync();

        List<Pomodoro> pomodoros = [];

        // Add pomodoros for completed tasks (2-6 per task)
        foreach (var task in doneTasks)
        {
            var pomodoroCount = Random.Shared.Next(2, 7);
            for (int i = 0; i < pomodoroCount; i++)
            {
                var startTime = task.CreatedOn.AddDays(Random.Shared.Next(0, 5)).AddHours(Random.Shared.Next(8, 18));
                pomodoros.Add(new Pomodoro
                {
                    TaskId = task.Id,
                    Task = task,
                    StartTime = startTime.DateTime,
                    DurationInMinutes = 25 // Standard pomodoro length
                });
            }
        }

        // Add some pomodoros for tasks in progress (1-3 per task)
        foreach (var task in doingTasks)
        {
            var pomodoroCount = Random.Shared.Next(1, 4);
            for (int i = 0; i < pomodoroCount; i++)
            {
                var startTime = task.CreatedOn.AddDays(Random.Shared.Next(1, 3)).AddHours(Random.Shared.Next(8, 18));
                pomodoros.Add(new Pomodoro
                {
                    TaskId = task.Id,
                    Task = task,
                    StartTime = startTime.DateTime,
                    DurationInMinutes = 25
                });
            }
        }

        // Set creation info for pomodoros
        foreach (var pomodoro in pomodoros)
        {
            pomodoro.SetCreationInfo();
            pomodoro.SetModificationInfo();
        }

        await context.Pomodoros.AddRangeAsync(pomodoros);
        await context.SaveChangesAsync();
    }
}