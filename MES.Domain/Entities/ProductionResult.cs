namespace MES.Domain.Entities;

public class ProductionResult
{
    public Guid Id { get; set; }

    public Guid ProductionLineId { get; set; }
    public Guid WorkOrderId { get; set; }

    public int GoodCount { get; set; }
    public int DefectCount { get; set; }

    // Qachon kiritildi
    public DateTime Timestamp { get; set; }
}
