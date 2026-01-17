using MES.Domain.Entities;
using MES.Domain.Enums;

namespace MES.Infrastructure.Persistence;

public static class AppDbContextSeed
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // ============================
        // 1) PRODUCTION LINES
        // ============================
        if (!context.ProductionLines.Any())
        {
            var lines = new List<ProductionLine>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Code = "LINE-1",
                    Name = "TV Assembly Line 1",
                    Status = ProductionLineStatus.Idle
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Code = "LINE-2",
                    Name = "TV Assembly Line 2",
                    Status = ProductionLineStatus.Idle
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Code = "LINE-3",
                    Name = "TV Assembly Line 3",
                    Status = ProductionLineStatus.Idle
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Code = "LINE-4",
                    Name = "TV Assembly Line 4",
                    Status = ProductionLineStatus.Idle
                }
            };

            context.ProductionLines.AddRange(lines);
            await context.SaveChangesAsync();
        }

        // Lines ni DB dan olamiz (har doim)
        var allLines = context.ProductionLines.ToList();

        // ============================
        // 2) STATIONS (har line uchun 5 ta)
        // ============================
        foreach (var line in allLines)
        {
            // Har bir station code bo‘yicha bor-yo‘qligini tekshiramiz
            await EnsureStationAsync(context,
                productionLineId: line.Id,
                code: $"{line.Code}-ST-01",
                name: "Screen & Barcode Scan",
                sequence: 1,
                type: StationType.Screen);

            await EnsureStationAsync(context,
                productionLineId: line.Id,
                code: $"{line.Code}-ST-02",
                name: "PCB Assembly",
                sequence: 2,
                type: StationType.PCB);

            await EnsureStationAsync(context,
                productionLineId: line.Id,
                code: $"{line.Code}-ST-03",
                name: "Functional Test",
                sequence: 3,
                type: StationType.Test);

            await EnsureStationAsync(context,
                productionLineId: line.Id,
                code: $"{line.Code}-ST-04",
                name: "Packing",
                sequence: 4,
                type: StationType.Pack);

            // ✅ SERVICE / REWORK (ST-05)
            await EnsureStationAsync(context,
                productionLineId: line.Id,
                code: $"{line.Code}-ST-05",
                name: "Service / Repair",
                sequence: 5,
                type: StationType.Service);
        }

        await context.SaveChangesAsync();

        // ============================
        // 3) WORK ORDER SEED (LINE-1 ga)
        // ============================
        if (!context.WorkOrders.Any())
        {
            var line1 = context.ProductionLines.First(x => x.Code == "LINE-1");

            var workOrder = new WorkOrder
            {
                Id = Guid.NewGuid(),
                ProductionLineId = line1.Id,
                ProductCode = "TV-55-QLED",
                PlannedQuantity = 100,
                ProducedQuantity = 0,

                PlannedStart = DateTime.UtcNow.AddHours(-1),
                PlannedEnd = DateTime.UtcNow.AddHours(7),

                IdealCycleTimeSeconds = 60, // 1 TV / 60 sec
                Status = WorkOrderStatus.Active
            };

            context.WorkOrders.Add(workOrder);
            await context.SaveChangesAsync();
        }
    }

    // ============================
    // Helper: stationni faqat yo‘q bo‘lsa qo‘shadi
    // ============================
    private static async Task EnsureStationAsync(
        AppDbContext context,
        Guid productionLineId,
        string code,
        string name,
        int sequence,
        StationType type)
    {
        // Code unique bo‘lgani uchun aynan code bilan tekshiramiz
        bool exists = context.Stations.Any(s => s.Code == code);
        if (exists) return;

        context.Stations.Add(new Station
        {
            Id = Guid.NewGuid(),
            ProductionLineId = productionLineId,
            Code = code,
            Name = name,
            Sequence = sequence,
            Type = type,
            IsManual = true
        });

        await Task.CompletedTask;
    }
}
