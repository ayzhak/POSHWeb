#nullable disable
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using POSHWeb.Enum;
using POSHWeb.Model;
using POSHWeb.Model.Job;
using POSHWeb.Model.Script;

namespace POSHWeb.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

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
        var stringArrayConverter =
            new ValueConverter<string[], string>(v => string.Join(";", v), v => v.Split(new[] {';'}));
        var intArrayConverter = new ValueConverter<int[], string>(
            v => string.Join(";", v),
            v => v.Split(";", StringSplitOptions.RemoveEmptyEntries).Select(val => int.Parse(val)).ToArray());
        var doubleArrayConverter = new ValueConverter<double[], string>(
            v => string.Join(";", v),
            v => v.Split(";", StringSplitOptions.RemoveEmptyEntries).Select(val => double.Parse(val)).ToArray());
        var floatArrayConverter = new ValueConverter<float[], string>(
            v => string.Join(";", v),
            v => v.Split(";", StringSplitOptions.RemoveEmptyEntries).Select(val => float.Parse(val)).ToArray());
        var booleanArrayConverter = new ValueConverter<bool[], string>(
            v => string.Join(";", v),
            v => v.Split(";", StringSplitOptions.RemoveEmptyEntries).Select(val => bool.Parse(val)).ToArray());
        var datetimeArrayConverter = new ValueConverter<DateTime[], string>(
            v => string.Join(";", v),
            v => v.Split(";", StringSplitOptions.RemoveEmptyEntries).Select(val => DateTime.Parse(val)).ToArray());
        var uintArrayConverter = new ValueConverter<uint[], string>(
            v => string.Join(";", v),
            v => v.Split(";", StringSplitOptions.RemoveEmptyEntries).Select(val => uint.Parse(val)).ToArray());
        var charArrayConverter = new ValueConverter<char[], string>(
            v => string.Join(";", v),
            v => v.Split(";", StringSplitOptions.RemoveEmptyEntries).Select(val => char.Parse(val)).ToArray());

        modelBuilder.Entity<PSParameterOptions>()
            .Property(nameof(PSParameterOptions.ValidValues))
            .HasConversion(stringArrayConverter);
        modelBuilder.Entity<PSParameterOptions>()
            .Property(nameof(PSParameterOptions.EnumValues))
            .HasConversion(stringArrayConverter);

        modelBuilder.Entity<InputParameter>()
            .Property(nameof(InputParameter.StringArray))
            .HasConversion(stringArrayConverter);
        modelBuilder.Entity<InputParameter>()
            .Property(nameof(InputParameter.IntegerArray))
            .HasConversion(intArrayConverter);
        modelBuilder.Entity<InputParameter>()
            .Property(nameof(InputParameter.DoubleArray))
            .HasConversion(doubleArrayConverter);
        modelBuilder.Entity<InputParameter>()
            .Property(nameof(InputParameter.FloatArray))
            .HasConversion(floatArrayConverter);
        modelBuilder.Entity<InputParameter>()
            .Property(nameof(InputParameter.BooleanArray))
            .HasConversion(booleanArrayConverter);
        modelBuilder.Entity<InputParameter>()
            .Property(nameof(InputParameter.DateTimeArray))
            .HasConversion(datetimeArrayConverter);
        modelBuilder.Entity<InputParameter>()
            .Property(nameof(InputParameter.CharArray))
            .HasConversion(charArrayConverter);
        modelBuilder.Entity<InputParameter>()
            .Property(nameof(InputParameter.UIntegerArray))
            .HasConversion(uintArrayConverter);

        modelBuilder.Entity<DefaultValue>()
            .Property(nameof(DefaultValue.StringArray))
            .HasConversion(stringArrayConverter);
        modelBuilder.Entity<DefaultValue>()
            .Property(nameof(DefaultValue.IntegerArray))
            .HasConversion(intArrayConverter);
        modelBuilder.Entity<DefaultValue>()
            .Property(nameof(DefaultValue.DoubleArray))
            .HasConversion(doubleArrayConverter);
        modelBuilder.Entity<DefaultValue>()
            .Property(nameof(DefaultValue.FloatArray))
            .HasConversion(floatArrayConverter);
        modelBuilder.Entity<DefaultValue>()
            .Property(nameof(DefaultValue.BooleanArray))
            .HasConversion(booleanArrayConverter);
        modelBuilder.Entity<DefaultValue>()
            .Property(nameof(DefaultValue.DateTimeArray))
            .HasConversion(datetimeArrayConverter);
        modelBuilder.Entity<DefaultValue>()
            .Property(nameof(DefaultValue.CharArray))
            .HasConversion(charArrayConverter);
        modelBuilder.Entity<DefaultValue>()
            .Property(nameof(DefaultValue.UIntegerArray))
            .HasConversion(uintArrayConverter);

        modelBuilder.Entity<PSHelp>()
            .Property(nameof(PSHelp.Examples))
            .HasConversion(stringArrayConverter);
        modelBuilder.Entity<PSHelp>()
            .Property(nameof(PSHelp.Inputs))
            .HasConversion(stringArrayConverter);
        modelBuilder.Entity<PSHelp>()
            .Property(nameof(PSHelp.Outputs))
            .HasConversion(stringArrayConverter);
        modelBuilder.Entity<PSHelp>()
            .Property(nameof(PSHelp.Links))
            .HasConversion(stringArrayConverter);
    }
}