namespace MES.Domain.Enums;

public enum ProductionLineStatus
{
    Idle = 0,     // Ish yo‘q
    Running = 1,  // WorkOrder ishlayapti
    Stopped = 2   // To‘xtatilgan (nosozlik, pauza)
}
