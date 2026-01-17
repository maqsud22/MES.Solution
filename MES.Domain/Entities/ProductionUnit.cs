namespace MES.Domain.Entities;

public class ProductionUnit
{
    public Guid Id { get; set; }

    // TV serial/barcode
    public string SerialNumber { get; set; } = null!;

    public Guid ProductionLineId { get; set; }
    public Guid WorkOrderId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // ✅ Qaysi bosqichgacha o‘tganini bildiradi
    public int CurrentSequence { get; set; } = 0;
    // 0 = hali boshlanmagan, 1..4 = qaysi bosqichdan o‘tgan

    // ✅ Test FAIL bo‘lsa servis rejimi
    public bool IsInService { get; set; } = false;

    // Qadoq bo‘lganda tugaydi
    public bool IsCompleted { get; set; }
}
