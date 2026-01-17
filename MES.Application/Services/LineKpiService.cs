using MES.Application.DTOs;
using MES.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MES.Application.Services;

public class LineKpiService
{
    private readonly IApplicationDbContext _context;

    public LineKpiService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LineKpiDto> GetLineKpiAsync(Guid productionLineId)
    {
        var workOrder = await _context.WorkOrders
            .Where(x => x.ProductionLineId == productionLineId)
            .OrderByDescending(x => x.ActualStart)
            .FirstOrDefaultAsync();

        if (workOrder == null || workOrder.ActualStart == null)
            return new LineKpiDto { ProductionLineId = productionLineId };

        var endTime = workOrder.ActualEnd ?? DateTime.UtcNow;

        var runTime = endTime - workOrder.ActualStart.Value;
        var plannedTime = workOrder.PlannedEnd - workOrder.PlannedStart;

        // 🔴 Downtime (shu WorkOrder bo‘yicha)
        var downtimeLogs = await _context.DowntimeLogs
            .Where(x => x.WorkOrderId == workOrder.Id)
            .ToListAsync();

        TimeSpan downtime = TimeSpan.Zero;
        foreach (var d in downtimeLogs)
        {
            var dEnd = d.EndTime ?? DateTime.UtcNow;
            downtime += (dEnd - d.StartTime);
        }

        var results = await _context.ProductionResults
            .Where(x => x.WorkOrderId == workOrder.Id)
            .ToListAsync();

        int totalProduced = results.Sum(x => x.GoodCount + x.DefectCount);
        int totalDefects = results.Sum(x => x.DefectCount);

        // QUALITY
        double qualityRate = totalProduced == 0
            ? 0
            : (double)(totalProduced - totalDefects) / totalProduced;

        // ✅ REAL AVAILABILITY = (RunTime - Downtime) / PlannedTime
        var operatingTime = runTime - downtime;
        if (operatingTime < TimeSpan.Zero) operatingTime = TimeSpan.Zero;

        double availabilityRate = plannedTime.TotalSeconds == 0
            ? 0
            : operatingTime.TotalSeconds / plannedTime.TotalSeconds;

        // PERFORMANCE (soddalashtirilgan)
        double idealCycleTimeSeconds = workOrder.IdealCycleTimeSeconds;
        double idealOutput = idealCycleTimeSeconds == 0
            ? 0
            : runTime.TotalSeconds / idealCycleTimeSeconds;

        double performanceRate = idealOutput == 0
            ? 0
            : totalProduced / idealOutput;

        // OEE
        double oee = availabilityRate * performanceRate * qualityRate;

        return new LineKpiDto
        {
            ProductionLineId = productionLineId,
            RunTime = runTime,
            PlannedTime = plannedTime,
            Downtime = downtime,
            TotalProduced = totalProduced,
            TotalDefects = totalDefects,
            QualityRate = qualityRate,
            AvailabilityRate = availabilityRate,
            PerformanceRate = performanceRate,
            Oee = oee
        };
    }

    public async Task<LineKpiDto> GetLineKpiByPeriodAsync(Guid productionLineId, DateTime from, DateTime to)
    {
        var workOrders = await _context.WorkOrders
            .Where(x =>
                x.ProductionLineId == productionLineId &&
                x.ActualStart != null &&
                x.ActualStart < to &&
                (x.ActualEnd ?? DateTime.UtcNow) > from)
            .ToListAsync();

        if (!workOrders.Any())
            return new LineKpiDto { ProductionLineId = productionLineId };

        TimeSpan runTime = TimeSpan.Zero;
        TimeSpan plannedTime = TimeSpan.Zero;
        TimeSpan downtime = TimeSpan.Zero;

        int totalProduced = 0;
        int totalDefects = 0;

        double idealTimeSum = 0;

        foreach (var wo in workOrders)
        {
            var start = wo.ActualStart!.Value < from ? from : wo.ActualStart.Value;
            var endRaw = wo.ActualEnd ?? DateTime.UtcNow;
            var end = endRaw > to ? to : endRaw;

            var woRunTime = end - start;
            runTime += woRunTime;

            plannedTime += (wo.PlannedEnd - wo.PlannedStart);

            // 🔴 Shu WorkOrder bo‘yicha downtime loglar
            var dLogs = await _context.DowntimeLogs
                .Where(x => x.WorkOrderId == wo.Id)
                .ToListAsync();

            foreach (var d in dLogs)
            {
                var dStart = d.StartTime < from ? from : d.StartTime;
                var dEndRaw = d.EndTime ?? DateTime.UtcNow;
                var dEnd = dEndRaw > to ? to : dEndRaw;

                if (dEnd > dStart)
                    downtime += (dEnd - dStart);
            }

            var results = await _context.ProductionResults
                .Where(x => x.WorkOrderId == wo.Id)
                .ToListAsync();

            int produced = results.Sum(x => x.GoodCount + x.DefectCount);
            int defects = results.Sum(x => x.DefectCount);

            totalProduced += produced;
            totalDefects += defects;

            idealTimeSum += wo.IdealCycleTimeSeconds * produced;
        }

        // QUALITY
        double quality = totalProduced == 0
            ? 0
            : (double)(totalProduced - totalDefects) / totalProduced;

        // ✅ REAL AVAILABILITY = (RunTime - Downtime) / PlannedTime
        var operatingTime = runTime - downtime;
        if (operatingTime < TimeSpan.Zero) operatingTime = TimeSpan.Zero;

        double availability = plannedTime.TotalSeconds == 0
            ? 0
            : operatingTime.TotalSeconds / plannedTime.TotalSeconds;

        // PERFORMANCE
        double performance = operatingTime.TotalSeconds == 0
            ? 0
            : idealTimeSum / operatingTime.TotalSeconds;

        // OEE
        double oee = availability * performance * quality;

        return new LineKpiDto
        {
            ProductionLineId = productionLineId,
            RunTime = runTime,
            PlannedTime = plannedTime,
            Downtime = downtime,
            TotalProduced = totalProduced,
            TotalDefects = totalDefects,
            QualityRate = quality,
            AvailabilityRate = availability,
            PerformanceRate = performance,
            Oee = oee
        };
    }
}
