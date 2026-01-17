namespace MES.Application.DTOs;

public class RecentUnitDto
{
    public string SerialNumber { get; set; } = "";
    public int CurrentSequence { get; set; }
    public bool IsInService { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime LastScanAt { get; set; }
}
