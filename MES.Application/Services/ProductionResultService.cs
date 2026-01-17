using MES.Application.Interfaces;
using MES.Domain.Entities;
using MES.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MES.Application.Services;

public class ProductionResultService
{
    private readonly IApplicationDbContext _context;

    public ProductionResultService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddResultAsync(
        Guid workOrderId,
        int goodCount,
        int defectCount)
    {
        if (goodCount < 0 || defectCount < 0)
            throw new Exception("Miqdor manfiy bo‘lishi mumkin emas");

        var workOrder = await _context.WorkOrders
            .FirstOrDefaultAsync(x => x.Id == workOrderId);

        if (workOrder == null)
            throw new Exception("WorkOrder topilmadi");

        if (workOrder.Status != WorkOrderStatus.Active)
            throw new Exception("Faqat Active WorkOrder ga natija yoziladi");

        var result = new ProductionResult
        {
            Id = Guid.NewGuid(),
            WorkOrderId = workOrderId,
            ProductionLineId = workOrder.ProductionLineId,
            GoodCount = goodCount,
            DefectCount = defectCount,
            Timestamp = DateTime.UtcNow
        };

        _context.ProductionResults.Add(result);

        // 🔴 JAMI ISHLAB CHIQARILGAN SONNI YANGILAYMIZ
        workOrder.ProducedQuantity += goodCount + defectCount;

        await _context.SaveChangesAsync();
    }
}
