using MES.Application.DTOs;
using MES.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MES.Application.Services;

public class UnitTraceService
{
    private readonly IApplicationDbContext _context;

    public UnitTraceService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UnitTraceDto?> GetBySerialAsync(string serial)
    {
        var unit = await _context.ProductionUnits
            .FirstOrDefaultAsync(x => x.SerialNumber == serial);

        if (unit == null) return null;

        var scans = await (from s in _context.StationScans
                           join st in _context.Stations on s.StationId equals st.Id
                           join op in _context.Operators on s.OperatorId equals op.Id into ops
                           from op in ops.DefaultIfEmpty()
                           where s.ProductionUnitId == unit.Id
                           orderby s.ScannedAt
                           select new UnitTraceScanDto
                           {
                               Sequence = st.Sequence,
                               StationName = st.Name,
                               ScannedAt = s.ScannedAt,
                               OperatorName = op != null ? op.FullName : null
                           })
                           .ToListAsync();

        return new UnitTraceDto
        {
            SerialNumber = unit.SerialNumber,
            ProductionLineId = unit.ProductionLineId,
            WorkOrderId = unit.WorkOrderId,
            CurrentSequence = unit.CurrentSequence,
            IsInService = unit.IsInService,
            IsCompleted = unit.IsCompleted,
            Scans = scans
        };
    }
}
