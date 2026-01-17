using MES.Application.DTOs;
using MES.Application.Interfaces;
using MES.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MES.Application.Services;

public class OperatorStatsService
{
    private readonly IApplicationDbContext _context;

    public OperatorStatsService(IApplicationDbContext context)
    {
        _context = context;
    }

    // ✅ 1) Bitta operator uchun kunlik statistika
    public async Task<OperatorDailyStatsDto> GetDailyAsync(Guid operatorId, DateTime dateUtc)
    {
        var from = dateUtc.Date;
        var to = from.AddDays(1);

        var op = await _context.Operators.FirstOrDefaultAsync(x => x.Id == operatorId);

        // total scans
        var totalScans = await _context.StationScans
            .Where(s => s.OperatorId == operatorId && s.ScannedAt >= from && s.ScannedAt < to)
            .CountAsync();

        // packed scans (JOIN Stations)  ✅ QAVS SHART!
        var packedCount = await (
            from s in _context.StationScans
            join st in _context.Stations on s.StationId equals st.Id
            where s.OperatorId == operatorId
                  && s.ScannedAt >= from && s.ScannedAt < to
                  && st.Type == StationType.Pack
            select s.Id
        ).CountAsync();

        // ❗Agar StationScan’da Result yo‘q bo‘lsa -> 0
        var testFailCount = 0;

        return new OperatorDailyStatsDto
        {
            OperatorId = operatorId,
            FullName = op?.FullName ?? "",
            TotalScans = totalScans,
            PackedCount = packedCount,
            TestFailCount = testFailCount
        };
    }

    // ✅ 2) BARCHA operatorlar bo‘yicha kunlik statistika
    public async Task<List<OperatorDailyStatsDto>> GetDailyAllAsync(DateTime dateUtc, Guid? lineId = null)
    {
        var from = dateUtc.Date;
        var to = from.AddDays(1);

        var q =
            from s in _context.StationScans
            join op in _context.Operators on s.OperatorId equals op.Id
            join st in _context.Stations on s.StationId equals st.Id
            where s.OperatorId != null
                  && s.ScannedAt >= from && s.ScannedAt < to
                  && (lineId == null || s.ProductionLineId == lineId)
            select new { op.Id, op.FullName, st.Type };

        var list = await q.ToListAsync();

        var result = list
            .GroupBy(x => new { x.Id, x.FullName })
            .Select(g => new OperatorDailyStatsDto
            {
                OperatorId = g.Key.Id,
                FullName = g.Key.FullName,
                TotalScans = g.Count(), // ❗Count() bo‘lishi shart (Count emas)
                PackedCount = g.Count(x => x.Type == StationType.Pack),
                TestFailCount = 0
            })
            .OrderByDescending(x => x.PackedCount)
            .ToList();

        return result;
    }
}
