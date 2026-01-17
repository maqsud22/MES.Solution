using MES.Domain.Enums; // 🔴 SHART


namespace MES.Domain.Entities;

public class WorkOrder
{
    public Guid Id { get; set; }

    // Masalan: TV-55-QLED
    public string ProductCode { get; set; } = null!;

    public int PlannedQuantity { get; set; }
    public int ProducedQuantity { get; set; }

    // 🔗 Qaysi liniyada ishlayapti
    public Guid ProductionLineId { get; set; }

    public DateTime PlannedStart { get; set; }
    public DateTime PlannedEnd { get; set; }

    public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Planned;

    public DateTime? ActualStart { get; set; }
    public DateTime? ActualEnd { get; set; }
    // 🔴 OEE uchun MUHIM
    // 1 dona mahsulotni ishlab chiqarish uchun ideal vaqt (sekund)
    public double IdealCycleTimeSeconds { get; set; }
}
