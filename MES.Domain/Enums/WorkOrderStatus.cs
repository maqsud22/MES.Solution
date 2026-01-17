namespace MES.Domain.Enums;

public enum WorkOrderStatus
{
    Planned = 0,   // Yaratilgan, lekin boshlanmagan
    Active = 1,    // Hozir ishlab chiqarilmoqda
    Paused = 2,    // Vaqtinchalik to‘xtagan
    Completed = 3 // Tugagan
}
