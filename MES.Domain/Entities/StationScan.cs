namespace MES.Domain.Entities;

public class StationScan
{
    public Guid Id { get; set; }

    public Guid ProductionLineId { get; set; }
    public Guid WorkOrderId { get; set; }

    public Guid StationId { get; set; }
    public Guid ProductionUnitId { get; set; }

    public DateTime ScannedAt { get; set; } = DateTime.UtcNow;

    public Guid? OperatorId { get; set; }   // ixtiyoriy
    public string? RawBarcode { get; set; } // scanner nimani yuborganini saqlash
}
