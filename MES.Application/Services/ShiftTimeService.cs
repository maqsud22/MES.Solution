using MES.Application.DTOs;

namespace MES.Application.Services;

public class ShiftTimeService
{
    // DAY:   09:00 - 18:00
    // NIGHT: 18:00 - 09:00 (ertasi kun)
    public ShiftRangeDto GetCurrentShiftRange(DateTime now)
    {
        var today = now.Date;

        var dayStart = today.AddHours(9);   // 09:00
        var dayEnd = today.AddHours(18);    // 18:00

        // DAY shift
        if (now >= dayStart && now < dayEnd)
        {
            return new ShiftRangeDto
            {
                From = dayStart,
                To = dayEnd,
                ShiftName = "DAY"
            };
        }

        // NIGHT shift
        // agar 20:00 dan keyin bo‘lsa: 20:00 today -> 08:00 tomorrow
        if (now >= dayEnd)
        {
            return new ShiftRangeDto
            {
                From = dayEnd,
                To = today.AddDays(1).AddHours(8),
                ShiftName = "NIGHT"
            };
        }

        // agar 00:00 - 08:00 oralig‘i bo‘lsa: kechagi 20:00 -> bugungi 08:00
        return new ShiftRangeDto
        {
            From = today.AddDays(-1).AddHours(20),
            To = dayStart,
            ShiftName = "NIGHT"
        };
    }
}
