using MES.Application.DTOs;
using MES.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MES.Application.Services;

public class OeeService
{
    private readonly IApplicationDbContext _context;

    public OeeService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LineOeeDto> GetOeeByPeriodAsync(Guid lineId, DateTime from, DateTime to)
    {
        if (to <= from)
            throw new ArgumentException("Period noto‘g‘ri (to <= from)");

        // 1) PlannedProductionTime = period
        var planned = to - from;

        // 2) Downtime (overlap bilan)
        var downtimes = await _context.DowntimeLogs
            .AsNoTracking()
            .Where(x => x.ProductionLineId == lineId
                        && x.StartTime < to
                        && (x.EndTime ?? DateTime.UtcNow) > from)
            .ToListAsync();

        var downtimeSeconds = 0.0;

        foreach (var d in downtimes)
        {
            var start = d.StartTime < from ? from : d.StartTime;
            var end = (d.EndTime ?? DateTime.UtcNow) > to ? to : (d.EndTime ?? DateTime.UtcNow);
            if (end > start)
                downtimeSeconds += (end - start).TotalSeconds;
        }

        var downtime = TimeSpan.FromSeconds(downtimeSeconds);

        // 3) OperatingTime
        var operatingSeconds = planned.TotalSeconds - downtime.TotalSeconds;
        if (operatingSeconds < 0) operatingSeconds = 0;
        var operating = TimeSpan.FromSeconds(operatingSeconds);

        // 4) ProductionResults (period ichida)
        var results = await _context.ProductionResults
            .AsNoTracking()
            .Where(x => x.ProductionLineId == lineId
                        && x.Timestamp >= from
                        && x.Timestamp <= to)
            .ToListAsync();

        var totalProduced = results.Sum(x => x.GoodCount + x.DefectCount);
        var totalDefects = results.Sum(x => x.DefectCount);

        // 5) IdealTimeSum (perioddagi WO lar bo‘yicha weighted)
        var workOrders = await _context.WorkOrders
            .AsNoTracking()
            .Where(x => x.ProductionLineId == lineId
                        && x.PlannedStart < to
                        && x.PlannedEnd > from)
            .ToListAsync();

        double idealTimeSumSeconds = 0;

        foreach (var wo in workOrders)
        {
            // shu WO ga tegishli produced (period ichida)
            var woProduced = results
                .Where(r => r.WorkOrderId == wo.Id)
                .Sum(r => r.GoodCount + r.DefectCount);

            if (wo.IdealCycleTimeSeconds > 0 && woProduced > 0)
                idealTimeSumSeconds += wo.IdealCycleTimeSeconds * woProduced;
        }

        // 6) Rates
        var availability = planned.TotalSeconds == 0
            ? 0
            : operating.TotalSeconds / planned.TotalSeconds;

        var performance = operating.TotalSeconds == 0
            ? 0
            : idealTimeSumSeconds / operating.TotalSeconds;

        var quality = totalProduced == 0
            ? 0
            : (double)(totalProduced - totalDefects) / totalProduced;

        // clamp 0..1 (xohlasang olib tashlaysan)
        availability = Clamp01(availability);
        performance = Clamp01(performance);
        quality = Clamp01(quality);

        var oee = availability * performance * quality;

        return new LineOeeDto
        {
            ProductionLineId = lineId,
            From = from,
            To = to,

            PlannedProductionTime = planned,
            Downtime = downtime,
            OperatingTime = operating,

            TotalProduced = totalProduced,
            TotalDefects = totalDefects,

            Availability = availability,
            Performance = performance,
            Quality = quality,
            Oee = oee
        };
    }

    private static double Clamp01(double v)
        => v < 0 ? 0 : (v > 1 ? 1 : v);
}
