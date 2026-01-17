using MES.Domain.Enums;

namespace MES.Application.DTOs;

public class LineOverviewDto
{
    public Guid LineId { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";

    public ProductionLineStatus Status { get; set; }

    // Active WorkOrder bo‘lsa
    public Guid? ActiveWorkOrderId { get; set; }
    public string? ProductCode { get; set; }

    public int PlannedQuantity { get; set; }
    public int ProducedQuantity { get; set; }

    // Progress: 0..1
    public double Progress { get; set; }

    // OEE snapshot
    public double Oee { get; set; }
    public double AvailabilityRate { get; set; }
    public double PerformanceRate { get; set; }
    public double QualityRate { get; set; }
}
