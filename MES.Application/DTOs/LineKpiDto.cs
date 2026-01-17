namespace MES.Application.DTOs;

public class LineKpiDto
{
    public Guid ProductionLineId { get; set; }

    // ⏱ Vaqtlar
    public TimeSpan RunTime { get; set; }
    public TimeSpan PlannedTime { get; set; }
    public TimeSpan Downtime { get; set; }

    // 📦 Ishlab chiqarish
    public int TotalProduced { get; set; }
    public int TotalDefects { get; set; }

    // 📊 OEE qismlari
    public double AvailabilityRate { get; set; }
    public double PerformanceRate { get; set; }
    public double QualityRate { get; set; }

    // 🔥 Yakuniy OEE
    public double Oee { get; set; }
}
