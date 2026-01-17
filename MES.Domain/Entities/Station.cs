using MES.Domain.Enums;

namespace MES.Domain.Entities;

public class Station
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;

    public Guid ProductionLineId { get; set; }

    // ✅ navigation qo‘sh
    public ProductionLine? ProductionLine { get; set; }

    public bool IsManual { get; set; } = true;
    // ✅ YANGI: ketma-ketlik (1..4)
    public int Sequence { get; set; }

    // ✅ YANGI: qaysi bosqich turi
    public StationType Type { get; set; }
}
