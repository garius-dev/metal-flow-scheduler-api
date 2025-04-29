using MetalFlowScheduler.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace MetalFlowScheduler.Api.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSet properties for all your entities
        public DbSet<OperationType> OperationTypes { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<WorkCenter> WorkCenters { get; set; }
        public DbSet<Line> Lines { get; set; }
        public DbSet<WorkCenterOperationRoute> WorkCentersOperationRoutes { get; set; }
        public DbSet<LineWorkCenterRoute> LinesWorkCentersRoutes { get; set; }
        public DbSet<ProductAvailablePerLine> ProductsAvailablesPerLines { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductOperationRoute> ProductsOperationRoutes { get; set; }
        public DbSet<SurplusPerProductAndWorkCenter> SurplusPerProductAndWorkCenters { get; set; }
        public DbSet<ProductionOrder> ProductionOrders { get; set; }
        public DbSet<ProductionOrderItem> ProductionOrderItems { get; set; }

        // Optional: Configure relationships or other model aspects using Fluent API
        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     base.OnModelCreating(modelBuilder);
        //     // Configure relationships, keys, etc. if not using Data Annotations or conventions
        // }

        public override Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow; // Use UtcNow for consistency

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        entry.Entity.LastUpdate = now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastUpdate = now;
                        // Prevent CreatedAt from being changed on modification
                        entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
