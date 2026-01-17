using MES.Domain.Enums;

namespace MES.Application.DTOs;

public class ScanRequestDto
{
    public Guid ProductionLineId { get; set; }
    public Guid WorkOrderId { get; set; }
    public Guid StationId { get; set; }

    public string SerialNumber { get; set; } = null!;

    // Test station uchun
    public ScanResult? Result { get; set; }

    public Guid? OperatorId { get; set; }
}
