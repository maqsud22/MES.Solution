namespace MES.Application.DTOs;

public class LineOeeDto
{
    public Guid ProductionLineId { get; set; }

    public DateTime From { get; set; }
    public DateTime To { get; set; }

    public TimeSpan PlannedProductionTime { get; set; }
    public TimeSpan Downtime { get; set; }
    public TimeSpan OperatingTime { get; set; }

    public int TotalProduced { get; set; }
    public int TotalDefects { get; set; }
    public int GoodCount => TotalProduced - TotalDefects;

    public double Availability { get; set; }
    public double Performance { get; set; }
    public double Quality { get; set; }
    public double Oee { get; set; }
}
