#nullable disable
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using POSHWeb.Model;
using POSHWeb.Model.Script;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;

namespace POSHWeb.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public Microsoft.EntityFrameworkCore.DbSet<PSScript> Script { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Job> Jobs { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<InputParameter> InputParameters { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<PSParameter> Parameters { get; set; }

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

                if (entity.State == EntityState.Added)
                {
                    ((BaseEntity) entity.Entity).CreatedAt = now;
                }

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
}