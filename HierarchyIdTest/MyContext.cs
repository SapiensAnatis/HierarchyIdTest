using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HierarchyIdTest;

internal sealed class MyContext : DbContext
{
    public DbSet<Service> Services { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer(
                $"Server=localhost;Password=YourStrong(!)Password123;User Id=sa;Encrypt=false;Initial Catalog=myDatabase",
                builder => builder.UseHierarchyId()
            )
            .EnableSensitiveDataLogging()
            .LogTo(
                s =>
                {
                    Console.WriteLine(s);
                },
                LogLevel.Information
            );
        ;
    }
}

public class Service
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public HierarchyId PathFromRoot { get; set; }
}
