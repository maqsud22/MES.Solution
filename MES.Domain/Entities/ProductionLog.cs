namespace MES.Domain.Entities;

public class ProductionLog
{
    public Guid Id { get; set; }

    public Guid ProductionLineId { get; set; }
    public Guid WorkOrderId { get; set; }

    // HOZIRCHA IXTiyoriy (keyin qo‘shamiz)
    public Guid? StationId { get; set; }
    public Guid? OperatorId { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public int GoodCount { get; set; }
    public int DefectCount { get; set; }

    public string Source { get; set; } = "MANUAL";
}
