using MES.Domain.Enums;

namespace MES.Domain.Entities;

public class ProductionLine
{
    public Guid Id { get; set; }

    // Masalan: LINE-1
    public string Code { get; set; } = null!;

    // Masalan: TV Assembly Line 1
    public string Name { get; set; } = null!;

    // 🔴 REAL ZAVOD HOLATI
    public ProductionLineStatus Status { get; set; }

    // Navigation (keyin kerak bo‘ladi)
    public ICollection<Station> Stations { get; set; } = new List<Station>();
}
