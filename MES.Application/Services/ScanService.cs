using MES.Application.DTOs;
using MES.Application.Interfaces;
using MES.Domain.Entities;
using MES.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MES.Application.Services;

public class ScanService
{
    private readonly IApplicationDbContext _context;

    public ScanService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task ScanAsync(ScanRequestDto dto)
    {
        // 1) Station
        var station = await _context.Stations
            .FirstOrDefaultAsync(x => x.Id == dto.StationId);

        if (station == null)
            throw new Exception("Station topilmadi");

        // 2) WorkOrder (Active bo‘lishi shart)
        var workOrder = await _context.WorkOrders
            .FirstOrDefaultAsync(x => x.Id == dto.WorkOrderId);

        if (workOrder == null || workOrder.Status != WorkOrderStatus.Active)
            throw new Exception("Active WorkOrder topilmadi");

        var op = await _context.Operators.FirstOrDefaultAsync(x => x.Id == dto.OperatorId);
        if (op == null || !op.IsActive)
            throw new Exception("Operator topilmadi yoki aktiv emas");

        // 3) Unit (serial)
        var unit = await _context.ProductionUnits
            .FirstOrDefaultAsync(x =>
                x.SerialNumber == dto.SerialNumber &&
                x.WorkOrderId == dto.WorkOrderId);

        if (unit == null)
        {
            // Faqat 1-bosqich yangi unit yaratishi mumkin
            if (station.Sequence != 1)
                throw new Exception("Bu TV hali 1-bosqichdan o‘tmagan");

            unit = new ProductionUnit
            {
                Id = Guid.NewGuid(),
                SerialNumber = dto.SerialNumber,
                ProductionLineId = dto.ProductionLineId,
                WorkOrderId = dto.WorkOrderId,
                CurrentSequence = 0,
                IsInService = false,
                IsCompleted = false
            };

            _context.ProductionUnits.Add(unit);
        }

        // 4) Completed bo‘lsa — blok
        if (unit.IsCompleted)
            throw new Exception("Bu TV allaqachon yakunlangan (PACK). Qayta scan mumkin emas.");

        // 5) SERVICE mode gatekeeping
        // Servisda bo‘lsa → faqat ST-05 (sequence=5, type=Service) ruxsat
        if (unit.IsInService)
        {
            if (station.Sequence != 5 || station.Type != StationType.Service)
                throw new Exception("Bu TV servisda. Faqat Service (ST-05) scan qilinadi.");
        }
        else
        {
            // Servisda bo‘lmasa → Service station scan qilinmasin
            if (station.Sequence == 5 || station.Type == StationType.Service)
                throw new Exception("Bu TV servisda emas. Service scan mumkin emas.");
        }

        // 6) Ketma-ketlik tekshiruvi (faqat servisda EMAS bo‘lsa)
        if (!unit.IsInService)
        {
            int expected = unit.CurrentSequence + 1;
            if (station.Sequence != expected)
                throw new Exception($"Oldingi bosqich o‘tilmagan. Kutilgan: {expected}");
        }

        // 7) Business logic (sequence update + service transitions)
        // CASE A: TEST (Seq=3)
        if (station.Sequence == 3 && station.Type == StationType.Test)
        {
            if (dto.Result == null)
                throw new Exception("Test natijasi (PASS/FAIL) kerak");

            if (dto.Result == ScanResult.Fail)
            {
                // FAIL → servisga yuboramiz
                unit.IsInService = true;

                // Keyingi kutilgan 5 bo‘lishi uchun 4 ga olib kelamiz (expected=5)
                unit.CurrentSequence = 4;
            }
            else
            {
                // PASS → normal davom
                unit.IsInService = false;
                unit.CurrentSequence = 3;
            }
        }
        // CASE B: SERVICE (Seq=5)
        else if (station.Sequence == 5 && station.Type == StationType.Service)
        {
            // Servis bajarildi → qayta testga qaytaramiz
            unit.IsInService = false;

            // Qayta TEST (3) qilish uchun CurrentSequence=2 qilamiz (expected=3)
            unit.CurrentSequence = 2;
        }
        // CASE C: NORMAL stations (Seq 1,2,4)
        else
        {
            unit.CurrentSequence = station.Sequence;

            // PACK bo‘lsa yakun
            if (station.Sequence == 4 && station.Type == StationType.Pack)
            {
                unit.IsCompleted = true;

                // ProductionResult yozamiz (1 ta tayyor TV)
                var result = new ProductionResult
                {
                    Id = Guid.NewGuid(),
                    ProductionLineId = dto.ProductionLineId,
                    WorkOrderId = dto.WorkOrderId,
                    GoodCount = 1,
                    DefectCount = 0,
                    Timestamp = DateTime.UtcNow
                };

                _context.ProductionResults.Add(result);
            }
        }
        // 8) Duplicate scan (real zavod qoidasi)
        // Screen/PCB/Pack da qayta scan mumkin emas
        if (station.Type != StationType.Test && station.Type != StationType.Service)
        {
            bool alreadyScanned = await _context.StationScans.AnyAsync(x =>
                x.ProductionUnitId == unit.Id &&
                x.StationId == station.Id);

            if (alreadyScanned)
                throw new Exception("Bu stationda bu TV allaqachon scan qilingan");
        }

        // Pack yakun bo‘lgani uchun u ham qayta scan bo‘lmasin
        if (station.Type == StationType.Pack && unit.IsCompleted)
            throw new Exception("Bu TV allaqachon PACK bo‘lgan");

        // 8) Scan log (har doim)
        var scan = new StationScan
        {
            Id = Guid.NewGuid(),
            ProductionLineId = dto.ProductionLineId,
            WorkOrderId = dto.WorkOrderId,
            StationId = station.Id,
            ProductionUnitId = unit.Id,
            OperatorId = dto.OperatorId,
            ScannedAt = DateTime.UtcNow,
            RawBarcode = dto.SerialNumber
        };

        _context.StationScans.Add(scan);

        await _context.SaveChangesAsync();
    }
}
