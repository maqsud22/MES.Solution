using MES.Application.Interfaces;
using MES.Domain.Entities;
using MES.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MES.Application.Services;

public class DowntimeService
{
    private readonly IApplicationDbContext _context;

    public DowntimeService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task StartDowntimeAsync(Guid workOrderId, DowntimeReason reason, string? comment = null)
    {
        var workOrder = await _context.WorkOrders
            .FirstOrDefaultAsync(x => x.Id == workOrderId);

        if (workOrder == null)
            throw new Exception("WorkOrder topilmadi");

        if (workOrder.Status != WorkOrderStatus.Active)
            throw new Exception("Faqat Active WorkOrder paytida downtime boshlanadi");

        // Ochiq downtime bo‘lmasligi kerak
        bool hasOpenDowntime = await _context.DowntimeLogs.AnyAsync(x =>
            x.WorkOrderId == workOrderId && x.EndTime == null);

        if (hasOpenDowntime)
            throw new Exception("Allaqachon ochiq downtime mavjud");

        var line = await _context.ProductionLines
            .FirstAsync(x => x.Id == workOrder.ProductionLineId);

        // Line stop holatiga o‘tadi
        line.Status = ProductionLineStatus.Stopped;

        var log = new DowntimeLog
        {
            Id = Guid.NewGuid(),
            ProductionLineId = workOrder.ProductionLineId,
            WorkOrderId = workOrder.Id,
            Reason = reason,
            StartTime = DateTime.UtcNow,
            EndTime = null,
            Comment = comment
        };

        _context.DowntimeLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task EndDowntimeAsync(Guid workOrderId)
    {
        var workOrder = await _context.WorkOrders
            .FirstOrDefaultAsync(x => x.Id == workOrderId);

        if (workOrder == null)
            throw new Exception("WorkOrder topilmadi");

        if (workOrder.Status != WorkOrderStatus.Active)
            throw new Exception("Faqat Active WorkOrder paytida downtime tugaydi");

        var log = await _context.DowntimeLogs
            .FirstOrDefaultAsync(x => x.WorkOrderId == workOrderId && x.EndTime == null);

        if (log == null)
            throw new Exception("Ochiq downtime topilmadi");

        log.EndTime = DateTime.UtcNow;

        var line = await _context.ProductionLines
            .FirstAsync(x => x.Id == workOrder.ProductionLineId);

        // Line yana Running holatiga qaytadi
        line.Status = ProductionLineStatus.Running;

        await _context.SaveChangesAsync();
    }
}
