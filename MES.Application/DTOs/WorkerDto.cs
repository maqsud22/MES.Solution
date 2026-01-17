namespace MES.Application.DTOs;

public class WorkerDto
{
    public Guid Id { get; set; }

    public string EmployeeCode { get; set; } = null!;
    public string FullName { get; set; } = null!;

    public string? BadgeCode { get; set; }
    public string? Position { get; set; }

    public Guid ProductionLineId { get; set; }
    public Guid StationId { get; set; }

    public bool IsActive { get; set; }
}
