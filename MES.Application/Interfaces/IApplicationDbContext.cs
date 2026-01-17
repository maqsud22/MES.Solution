using Microsoft.EntityFrameworkCore;
using MES.Domain.Entities;

namespace MES.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<ProductionLine> ProductionLines { get; }
    DbSet<Station> Stations { get; }
    DbSet<Operator> Operators { get; }
    DbSet<WorkOrder> WorkOrders { get; }
    DbSet<ProductionLog> ProductionLogs { get; }
    DbSet<ProductionResult> ProductionResults { get; }
    DbSet<DowntimeLog> DowntimeLogs { get; }
    DbSet<ProductionUnit> ProductionUnits { get; }
    DbSet<StationScan> StationScans { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}