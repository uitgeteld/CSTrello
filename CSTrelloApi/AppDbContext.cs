using CSTrelloApi.Data;
using Microsoft.EntityFrameworkCore;

namespace CSTrelloApi;

public class AppDbContext:DbContext
{
    public DbSet<Tasks> Tasks { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql("server=localhost;port=3306;database=Vehicles;user=root;password=;",ServerVersion.Parse("8.0.30"));
    }
}