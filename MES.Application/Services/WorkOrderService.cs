using MES.Application.Interfaces;
using MES.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MES.Application.Services;

public class WorkOrderService
{
    private readonly IApplicationDbContext _context;

    public WorkOrderService(IApplicationDbContext context)
    {
        _context = context;
    }

    // ============================
    // START WORK ORDER
    // ============================
    public async Task StartWorkOrderAsync(Guid workOrderId)
    {
        var workOrder = await _context.WorkOrders
            .FirstOrDefaultAsync(x => x.Id == workOrderId);

        if (workOrder == null)
            throw new Exception("WorkOrder topilmadi");

        // 🔴 Faqat Planned holatdagisi ishga tushadi
        if (workOrder.Status != WorkOrderStatus.Planned)
            throw new Exception("Faqat Planned holatdagi WorkOrder ishga tushiriladi");

        // 🔴 1 ta liniyada 1 ta Active WorkOrder bo‘lishi shart
        bool hasActive = await _context.WorkOrders.AnyAsync(x =>
            x.ProductionLineId == workOrder.ProductionLineId &&
            x.Status == WorkOrderStatus.Active);

        if (hasActive)
            throw new Exception("Bu liniyada allaqachon active WorkOrder bor");

        // 🔴 Liniyani olamiz
        var line = await _context.ProductionLines
            .FirstAsync(x => x.Id == workOrder.ProductionLineId);

        // 🔴 Holatlarni o‘zgartiramiz
        workOrder.Status = WorkOrderStatus.Active;
        workOrder.ActualStart = DateTime.UtcNow;

        line.Status = ProductionLineStatus.Running;

        await _context.SaveChangesAsync();
    }

    // ============================
    // STOP WORK ORDER
    // ============================
    public async Task StopWorkOrderAsync(Guid workOrderId)
    {
        var workOrder = await _context.WorkOrders
            .FirstOrDefaultAsync(x => x.Id == workOrderId);

        if (workOrder == null)
            throw new Exception("WorkOrder topilmadi");

        // 🔴 Faqat Active WorkOrder to‘xtatiladi
        if (workOrder.Status != WorkOrderStatus.Active)
            throw new Exception("Faqat Active WorkOrder to‘xtatiladi");

        // 🔴 OCHIQ PRODUCTION LOGNI TOPAMIZ
        var log = await _context.ProductionLogs
            .FirstOrDefaultAsync(x =>
                x.WorkOrderId == workOrder.Id &&
                x.EndTime == null);

        if (log == null)
            throw new Exception("Ochiq ProductionLog topilmadi");

        // 🔴 Liniyani olamiz
        var line = await _context.ProductionLines
            .FirstAsync(x => x.Id == workOrder.ProductionLineId);

        // 🔴 HOLATLARNI YOPAMIZ
        workOrder.Status = WorkOrderStatus.Completed;
        workOrder.ActualEnd = DateTime.UtcNow;

        log.EndTime = DateTime.UtcNow;
        line.Status = ProductionLineStatus.Idle;

        await _context.SaveChangesAsync();
    }
}
