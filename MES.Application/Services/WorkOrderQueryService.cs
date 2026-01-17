using MES.Application.DTOs;
using MES.Application.Interfaces;
using MES.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MES.Application.Services;

public class WorkOrderQueryService
{
    private readonly IApplicationDbContext _context;
    public WorkOrderQueryService(IApplicationDbContext context) => _context = context;

    public async Task<ActiveWorkOrderDto> GetActiveByLineAsync(Guid lineId)
    {
        var wo = await _context.WorkOrders
            .Where(x => x.ProductionLineId == lineId && x.Status == WorkOrderStatus.Active)
            .OrderByDescending(x => x.ActualStart)
            .FirstOrDefaultAsync();

        if (wo == null)
            return new ActiveWorkOrderDto();

        // ProducedQuantity ni ProductionResults dan hisoblaymiz (eng ishonchli)
        var produced = await _context.ProductionResults
            .Where(x => x.WorkOrderId == wo.Id)
            .SumAsync(x => x.GoodCount + x.DefectCount);

        return new ActiveWorkOrderDto
        {
            WorkOrderId = wo.Id,
            ProductCode = wo.ProductCode,
            PlannedQuantity = wo.PlannedQuantity,
            ProducedQuantity = produced,
            Status = wo.Status,
            ActualStart = wo.ActualStart
        };
    }
}
