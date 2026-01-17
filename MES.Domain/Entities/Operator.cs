namespace MES.Domain.Entities;

public class Operator
{
    public Guid Id { get; set; }

    public string EmployeeCode { get; set; } = null!;
    public string FullName { get; set; } = null!;

    public string? BadgeCode { get; set; }   // NEW
    public string? Position { get; set; }    // NEW

    public Guid ProductionLineId { get; set; }
    public Guid StationId { get; set; }

    public bool IsActive { get; set; }
}
