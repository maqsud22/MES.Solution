namespace MES.Application.DTOs;

public class OperatorDailyStatsDto
{
    public Guid OperatorId { get; set; }
    public string FullName { get; set; } = "";

    public int TotalScans { get; set; }
    public int PackedCount { get; set; }
    public int TestFailCount { get; set; } // hozircha 0 (StationScan’da Result yo‘q bo‘lsa)
}
