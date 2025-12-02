using Microsoft.EntityFrameworkCore;
using Task = CSTrelloApi.Data.Task;

namespace CSTrelloApi;

public class AppDbContext:DbContext
{
    public DbSet<Task> Tasks { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql("server=localhost;port=3306;database=Trello;user=root;password=;",ServerVersion.Parse("8.0.30"));
    }
}