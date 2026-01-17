namespace MES.Application.DTOs;

public class ShiftRangeDto
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public string ShiftName { get; set; } = "DAY"; // DAY / NIGHT
}
