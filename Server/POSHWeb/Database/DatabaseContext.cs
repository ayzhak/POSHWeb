#nullable disable
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using POSHWeb.Model;
using POSHWeb.Model.Script;

namespace POSHWeb.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public DbSet<PSScript> Script { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<InputParameter> InputParameters { get; set; }
    public DbSet<PSParameter> Parameters { get; set; }

    public override int SaveChanges()
    {
        AddTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void AddTimestamps()
    {
        var entities = ChangeTracker.Entries()
            .Where(x => x.Entity is BaseEntity &&
                        (x.State == EntityState.Added || x.State == EntityState.Modified));

        foreach (var entity in entities)
        {
            var now = DateTime.UtcNow; // current datetime

            if (entity.State == EntityState.Added) ((BaseEntity) entity.Entity).CreatedAt = now;

            ((BaseEntity) entity.Entity).UpdatedAt = now;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var splitStringConverter =
            new ValueConverter<string[], string>(v => string.Join(";", v), v => v.Split(new[] {';'}));
        modelBuilder.Entity<PSParameterOptions>()
            .Property(nameof(PSParameterOptions.ValidValues))
            .HasConversion(splitStringConverter);
    }
}