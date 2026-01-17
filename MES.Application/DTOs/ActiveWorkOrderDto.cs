using MES.Domain.Enums;

namespace MES.Application.DTOs;

public class ActiveWorkOrderDto
{
    public Guid? WorkOrderId { get; set; }
    public string? ProductCode { get; set; }
    public int PlannedQuantity { get; set; }
    public int ProducedQuantity { get; set; }
    public WorkOrderStatus? Status { get; set; }
    public DateTime? ActualStart { get; set; }
}
