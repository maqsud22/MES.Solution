using MES.Domain.Enums;

namespace MES.Domain.Entities;

public class DowntimeLog
{
    public Guid Id { get; set; }

    public Guid ProductionLineId { get; set; }
    public Guid WorkOrderId { get; set; }

    public DowntimeReason Reason { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public string? Comment { get; set; }
}
