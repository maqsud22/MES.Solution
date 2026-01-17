using MES.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MES.Solution.Controllers;

[ApiController]
[Route("api/lines/{lineId:guid}/oee")]
public class OeeController : ControllerBase
{
    private readonly OeeService _oeeService;
    private readonly ShiftTimeService _shiftTimeService;

    public OeeController(
        OeeService oeeService,
        ShiftTimeService shiftTimeService)
    {
        _oeeService = oeeService;
        _shiftTimeService = shiftTimeService;
    }

    // ✅ CURRENT SHIFT OEE
    [HttpGet("shift/current")]
    public async Task<IActionResult> GetCurrentShift(Guid lineId)
    {
        var now = DateTime.UtcNow; // muhim
        var shift = _shiftTimeService.GetCurrentShiftRange(now);

        var dto = await _oeeService.GetOeeByPeriodAsync(
            lineId,
            shift.From,
            shift.To
        );

        return Ok(dto);
    }

    // ✅ DAILY (BUGUN)
    [HttpGet("daily")]
    public async Task<IActionResult> GetDaily(Guid lineId)
    {
        var today = DateTime.UtcNow.Date;

        var dto = await _oeeService.GetOeeByPeriodAsync(
            lineId,
            today,
            today.AddDays(1)
        );

        return Ok(dto);
    }

    // ✅ MONTHLY
    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthly(Guid lineId)
    {
        var now = DateTime.UtcNow;
        var from = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var to = from.AddMonths(1);

        var dto = await _oeeService.GetOeeByPeriodAsync(lineId, from, to);
        return Ok(dto);
    }

    // ✅ CUSTOM PERIOD
    [HttpGet("period")]
    public async Task<IActionResult> GetByPeriod(
        Guid lineId,
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        var dto = await _oeeService.GetOeeByPeriodAsync(
            lineId,
            from.ToUniversalTime(),
            to.ToUniversalTime()
        );

        return Ok(dto);
    }
}
