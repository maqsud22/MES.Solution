using MES.Application.Interfaces;
using MES.Domain.Entities;
using MES.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MES.Application.Services;

public class ProductionLogService
{
    private readonly IApplicationDbContext _context;

    public ProductionLogService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddProductionResultAsync(
        Guid workOrderId,
        int goodCount,
        int defectCount,
        string source = "MANUAL")
    {
        // 🔴 Active WorkOrder ni topamiz
        var workOrder = await _context.WorkOrders
            .FirstOrDefaultAsync(x => x.Id == workOrderId);

        if (workOrder == null)
            throw new Exception("WorkOrder topilmadi");

        if (workOrder.Status != WorkOrderStatus.Active)
            throw new Exception("Faqat Active WorkOrder uchun yozish mumkin");

        // 🔴 Ochiq ProductionLog ni topamiz
        var log = await _context.ProductionLogs
            .FirstOrDefaultAsync(x =>
                x.WorkOrderId == workOrderId &&
                x.EndTime == null);

        if (log == null)
            throw new Exception("Ochiq ProductionLog topilmadi");

        // 🔴 HISOBLARNI QO‘SHAMIZ
        log.GoodCount += goodCount;
        log.DefectCount += defectCount;

        log.Source = source;

        await _context.SaveChangesAsync();
    }
}
