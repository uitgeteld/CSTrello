using DevOne.Security.Cryptography.BCrypt;
using Microsoft.EntityFrameworkCore;
using Task = CSTrelloApi.Data.Task;

namespace CSTrelloApi;

public class AppDbContext:DbContext
{
    public DbSet<Task> Tasks { get; set; }
    public DbSet<Data.User> Users { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql("server=localhost;port=3306;database=Trello;user=root;password=;",ServerVersion.Parse("8.0.30"));
    }
protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    
        // Seed Users
        modelBuilder.Entity<Data.User>().HasData(
            new Data.User { Id = 1, Name = "Alice Johnson", Email = "alice.johnson@example.com", Password = BCryptHelper.HashPassword("password123", BCryptHelper.GenerateSalt()) },
            new Data.User { Id = 2, Name = "Bob Smith", Email = "bob.smith@example.com", Password = BCryptHelper.HashPassword("securePass456", BCryptHelper.GenerateSalt()) },
            new Data.User { Id = 3, Name = "Carol White", Email = "carol.white@example.com", Password = BCryptHelper.HashPassword("carol789", BCryptHelper.GenerateSalt()) },
            new Data.User { Id = 4, Name = "David Brown", Email = "david.brown@example.com", Password = BCryptHelper.HashPassword("david2024", BCryptHelper.GenerateSalt()) },
            new Data.User { Id = 5, Name = "Emma Davis", Email = "emma.davis@example.com", Password = BCryptHelper.HashPassword("emmaPass", BCryptHelper.GenerateSalt()) },
            new Data.User { Id = 6, Name = "Frank Miller", Email = "frank.miller@example.com", Password = BCryptHelper.HashPassword("frank555", BCryptHelper.GenerateSalt()) },
            new Data.User { Id = 7, Name = "Grace Wilson", Email = "grace.wilson@example.com", Password = BCryptHelper.HashPassword("grace999", BCryptHelper.GenerateSalt()) },
            new Data.User { Id = 8, Name = "Henry Moore", Email = "henry.moore@example.com", Password = BCryptHelper.HashPassword("henry2023", BCryptHelper.GenerateSalt()) },
            new Data.User { Id = 9, Name = "Ivy Taylor", Email = "ivy.taylor@example.com", Password = BCryptHelper.HashPassword("ivySecure", BCryptHelper.GenerateSalt()) },
            new Data.User { Id = 10, Name = "Jack Anderson", Email = "jack.anderson@example.com", Password = BCryptHelper.HashPassword("jack1234", BCryptHelper.GenerateSalt()) }
        );
    
        // Seed Tasks
        modelBuilder.Entity<Task>().HasData(
            new Task { Id = 1, Title = "Design Homepage", Description = "Create mockups for the new homepage", Status = "In Progress", AssignedTo = "alice.johnson@example.com" },
            new Task { Id = 2, Title = "Implement Authentication", Description = "Add user login and registration", Status = "To Do", AssignedTo = "bob.smith@example.com" },
            new Task { Id = 3, Title = "Database Migration", Description = "Update database schema for new features", Status = "Completed", AssignedTo = "carol.white@example.com" },
            new Task { Id = 4, Title = "Write Unit Tests", Description = "Create tests for API endpoints", Status = "In Progress", AssignedTo = "david.brown@example.com" },
            new Task { Id = 5, Title = "Fix Login Bug", Description = "Resolve issue with password validation", Status = "To Do", AssignedTo = "emma.davis@example.com" },
            new Task { Id = 6, Title = "Update Documentation", Description = "Document new API endpoints", Status = "Completed", AssignedTo = "frank.miller@example.com" },
            new Task { Id = 7, Title = "Optimize Queries", Description = "Improve database query performance", Status = "In Progress", AssignedTo = "grace.wilson@example.com" },
            new Task { Id = 8, Title = "Setup CI/CD", Description = "Configure continuous integration pipeline", Status = "To Do", AssignedTo = "henry.moore@example.com" },
            new Task { Id = 9, Title = "Code Review", Description = "Review pull requests from team members", Status = "In Progress", AssignedTo = "ivy.taylor@example.com" },
            new Task { Id = 10, Title = "Deploy to Production", Description = "Push latest changes to production server", Status = "To Do", AssignedTo = "jack.anderson@example.com" }
        );
    }
    
}