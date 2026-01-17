using Microsoft.EntityFrameworkCore;
using MES.Domain.Entities;
using MES.Application.Interfaces;

namespace MES.Infrastructure.Persistence;

public class AppDbContext : DbContext, IApplicationDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<ProductionLine> ProductionLines => Set<ProductionLine>();
    public DbSet<Station> Stations => Set<Station>();
    public DbSet<Operator> Operators => Set<Operator>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<ProductionLog> ProductionLogs => Set<ProductionLog>();
    public DbSet<ProductionResult> ProductionResults => Set<ProductionResult>();
    public DbSet<DowntimeLog> DowntimeLogs => Set<DowntimeLog>();
    public DbSet<ProductionUnit> ProductionUnits => Set<ProductionUnit>();
    public DbSet<StationScan> StationScans => Set<StationScan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
