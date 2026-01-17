using MES.Application.DTOs;
using MES.Application.Interfaces;
using MES.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MES.Application.Services;

public class WorkerService
{
    private readonly IApplicationDbContext _context;

    public WorkerService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<WorkerDto>> GetAllAsync(bool? isActive = null)
    {
        var q = _context.Operators.AsQueryable();

        if (isActive != null)
            q = q.Where(x => x.IsActive == isActive.Value);

        return await q
            .OrderBy(x => x.EmployeeCode)
            .Select(x => new WorkerDto
            {
                Id = x.Id,
                EmployeeCode = x.EmployeeCode,
                FullName = x.FullName,
                BadgeCode = x.BadgeCode,
                Position = x.Position,
                ProductionLineId = x.ProductionLineId,
                StationId = x.StationId,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public async Task<WorkerDto?> GetByIdAsync(Guid id)
    {
        return await _context.Operators
            .Where(x => x.Id == id)
            .Select(x => new WorkerDto
            {
                Id = x.Id,
                EmployeeCode = x.EmployeeCode,
                FullName = x.FullName,
                BadgeCode = x.BadgeCode,
                Position = x.Position,
                ProductionLineId = x.ProductionLineId,
                StationId = x.StationId,
                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Guid> CreateAsync(WorkerCreateDto dto)
    {
        dto.EmployeeCode = dto.EmployeeCode.Trim();
        dto.FullName = dto.FullName.Trim();
        dto.BadgeCode = dto.BadgeCode?.Trim();
        dto.Position = dto.Position?.Trim();

        // EmployeeCode unique
        bool empExists = await _context.Operators.AnyAsync(x => x.EmployeeCode == dto.EmployeeCode);
        if (empExists) throw new Exception("EmployeeCode allaqachon mavjud");

        // BadgeCode unique (agar berilgan bo‘lsa)
        if (!string.IsNullOrWhiteSpace(dto.BadgeCode))
        {
            bool badgeExists = await _context.Operators.AnyAsync(x => x.BadgeCode == dto.BadgeCode);
            if (badgeExists) throw new Exception("BadgeCode allaqachon mavjud");
        }

        // Line va Station mavjudligini tekshiramiz (xatolarni oldini oladi)
        bool lineOk = await _context.ProductionLines.AnyAsync(x => x.Id == dto.ProductionLineId);
        if (!lineOk) throw new Exception("ProductionLine topilmadi");

        bool stationOk = await _context.Stations.AnyAsync(x => x.Id == dto.StationId && x.ProductionLineId == dto.ProductionLineId);
        if (!stationOk) throw new Exception("Station topilmadi (yoki bu line ga tegishli emas)");

        var worker = new Operator
        {
            Id = Guid.NewGuid(),
            EmployeeCode = dto.EmployeeCode,
            FullName = dto.FullName,
            BadgeCode = dto.BadgeCode,
            Position = dto.Position,
            ProductionLineId = dto.ProductionLineId,
            StationId = dto.StationId,
            IsActive = dto.IsActive
        };

        _context.Operators.Add(worker);
        await _context.SaveChangesAsync();

        return worker.Id;
    }

    public async Task UpdateAsync(Guid id, WorkerUpdateDto dto)
    {
        dto.EmployeeCode = dto.EmployeeCode.Trim();
        dto.FullName = dto.FullName.Trim();
        dto.BadgeCode = dto.BadgeCode?.Trim();
        dto.Position = dto.Position?.Trim();

        var worker = await _context.Operators.FirstOrDefaultAsync(x => x.Id == id);
        if (worker == null) throw new Exception("Worker topilmadi");

        // EmployeeCode unique (other rows)
        bool empExists = await _context.Operators.AnyAsync(x => x.EmployeeCode == dto.EmployeeCode && x.Id != id);
        if (empExists) throw new Exception("EmployeeCode allaqachon mavjud");

        // BadgeCode unique (other rows)
        if (!string.IsNullOrWhiteSpace(dto.BadgeCode))
        {
            bool badgeExists = await _context.Operators.AnyAsync(x => x.BadgeCode == dto.BadgeCode && x.Id != id);
            if (badgeExists) throw new Exception("BadgeCode allaqachon mavjud");
        }

        bool lineOk = await _context.ProductionLines.AnyAsync(x => x.Id == dto.ProductionLineId);
        if (!lineOk) throw new Exception("ProductionLine topilmadi");

        bool stationOk = await _context.Stations.AnyAsync(x => x.Id == dto.StationId && x.ProductionLineId == dto.ProductionLineId);
        if (!stationOk) throw new Exception("Station topilmadi (yoki bu line ga tegishli emas)");

        worker.EmployeeCode = dto.EmployeeCode;
        worker.FullName = dto.FullName;
        worker.BadgeCode = dto.BadgeCode;
        worker.Position = dto.Position;
        worker.ProductionLineId = dto.ProductionLineId;
        worker.StationId = dto.StationId;
        worker.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();
    }

    // Soft delete = deactivate
    public async Task DeactivateAsync(Guid id)
    {
        var worker = await _context.Operators.FirstOrDefaultAsync(x => x.Id == id);
        if (worker == null) throw new Exception("Worker topilmadi");

        worker.IsActive = false;
        await _context.SaveChangesAsync();
    }
}
