using MES.Application.DTOs;
using MES.Application.Interfaces;
using MES.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MES.Application.Services;

public class LineOverviewService
{
    private readonly IApplicationDbContext _context;
    private readonly LineKpiService _kpiService;

    public LineOverviewService(IApplicationDbContext context, LineKpiService kpiService)
    {
        _context = context;
        _kpiService = kpiService;
    }

    public async Task<List<LineOverviewDto>> GetAllAsync()
    {
        var lines = await _context.ProductionLines
            .OrderBy(x => x.Code)
            .ToListAsync();

        var result = new List<LineOverviewDto>();

        foreach (var line in lines)
        {
            var activeWo = await _context.WorkOrders
                .Where(x => x.ProductionLineId == line.Id && x.Status == WorkOrderStatus.Active)
                .OrderByDescending(x => x.ActualStart)
                .FirstOrDefaultAsync();

            double progress = 0;
            if (activeWo != null && activeWo.PlannedQuantity > 0)
            {
                // ProducedQuantity hozir birma-bir yangilanmagan bo‘lishi mumkin,
                // shuning uchun ProductionResults’dan hisoblaymiz
                var produced = await _context.ProductionResults
                    .Where(x => x.WorkOrderId == activeWo.Id)
                    .SumAsync(x => x.GoodCount + x.DefectCount);

                progress = Math.Min(1.0, (double)produced / activeWo.PlannedQuantity);
            }

            var kpi = await _kpiService.GetLineKpiAsync(line.Id);

            result.Add(new LineOverviewDto
            {
                LineId = line.Id,
                Code = line.Code,
                Name = line.Name,
                Status = line.Status,

                ActiveWorkOrderId = activeWo?.Id,
                ProductCode = activeWo?.ProductCode,
                PlannedQuantity = activeWo?.PlannedQuantity ?? 0,

                // ProducedQuantity’ni KPI’dan olib turamiz
                ProducedQuantity = kpi.TotalProduced,
                Progress = progress,

                Oee = kpi.Oee,
                AvailabilityRate = kpi.AvailabilityRate,
                PerformanceRate = kpi.PerformanceRate,
                QualityRate = kpi.QualityRate
            });
        }

        return result;
    }
}
