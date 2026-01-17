namespace MES.Application.DTOs;

public class UnitTraceDto
{
    public string SerialNumber { get; set; } = "";
    public Guid ProductionLineId { get; set; }
    public Guid WorkOrderId { get; set; }

    public int CurrentSequence { get; set; }
    public bool IsInService { get; set; }
    public bool IsCompleted { get; set; }

    public List<UnitTraceScanDto> Scans { get; set; } = new();
}

public class UnitTraceScanDto
{
    public int Sequence { get; set; }
    public string StationName { get; set; } = "";
    public DateTime ScannedAt { get; set; }
    public string? OperatorName { get; set; }
}
