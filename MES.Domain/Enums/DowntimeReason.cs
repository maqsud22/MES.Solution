namespace MES.Domain.Enums;

public enum DowntimeReason
{
    Unknown = 0,

    // 🔧 Texnik
    MachineFailure = 1,
    PowerFailure = 2,

    // 👤 Insoniy
    OperatorBreak = 10,
    NoOperator = 11,

    // 📦 Material
    NoMaterial = 20,
    QualityIssue = 21,

    // 🛠 Rejalashtirilgan
    PlannedMaintenance = 30
}
