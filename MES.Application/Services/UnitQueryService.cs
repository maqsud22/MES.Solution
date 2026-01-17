using MES.Application.DTOs;
using MES.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MES.Application.Services;

public class UnitQueryService
{
    private readonly IApplicationDbContext _context;
    public UnitQueryService(IApplicationDbContext context) => _context = context;

    public async Task<List<RecentUnitDto>> GetRecentByLineAsync(Guid lineId, int take = 50)
    {
        // Oxirgi scan vaqtini StationScans dan olamiz
        var query =
            from u in _context.ProductionUnits.AsNoTracking()
            join s in _context.StationScans.AsNoTracking() on u.Id equals s.ProductionUnitId into scans
            where u.ProductionLineId == lineId
            let lastScan = scans.OrderByDescending(x => x.ScannedAt).FirstOrDefault()
            orderby lastScan != null ? lastScan.ScannedAt : DateTime.MinValue descending
            select new RecentUnitDto
            {
                SerialNumber = u.SerialNumber,
                CurrentSequence = u.CurrentSequence,
                IsInService = u.IsInService,
                IsCompleted = u.IsCompleted,
                LastScanAt = lastScan != null ? lastScan.ScannedAt : DateTime.MinValue
            };

        return await query.Take(take).ToListAsync();
    }
}
