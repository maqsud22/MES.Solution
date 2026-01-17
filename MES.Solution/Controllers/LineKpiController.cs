using Microsoft.AspNetCore.Mvc;
using MES.Application.Services;

namespace MES.Solution.Controllers;

[ApiController]
[Route("api/lines")]
public class LineKpiController : ControllerBase
{
    private readonly LineKpiService _lineKpiService;
    private readonly ShiftTimeService _shiftTimeService;

    public LineKpiController(LineKpiService lineKpiService, ShiftTimeService shiftTimeService)
    {
        _lineKpiService = lineKpiService;
        _shiftTimeService = shiftTimeService;
    }

    // ✅ Hozirgi (oxirgi WorkOrder bo‘yicha) KPI
    [HttpGet("{lineId:guid}/kpi/current")]
    public async Task<IActionResult> GetCurrentKpi(Guid lineId)
    {
        var kpi = await _lineKpiService.GetLineKpiAsync(lineId);
        return Ok(kpi);
    }

    // ✅ Hozirgi smena bo‘yicha KPI (08:00–20:00 / 20:00–08:00)
    [HttpGet("{lineId:guid}/kpi/shift/current")]
    public async Task<IActionResult> GetCurrentShiftKpi(Guid lineId)
    {
        var shift = _shiftTimeService.GetCurrentShiftRange(DateTime.Now);
        var kpi = await _lineKpiService.GetLineKpiByPeriodAsync(lineId, shift.From, shift.To);

        return Ok(new
        {
            shift.ShiftName,
            shift.From,
            shift.To,
            kpi
        });
    }

    // ✅ Kunlik KPI (bugun)
    [HttpGet("{lineId:guid}/kpi/daily")]
    public async Task<IActionResult> GetDailyKpi(Guid lineId)
    {
        var from = DateTime.Today;
        var to = DateTime.Today.AddDays(1);

        var kpi = await _lineKpiService.GetLineKpiByPeriodAsync(lineId, from, to);
        return Ok(kpi);
    }

    // ✅ Oylik KPI (year/month query)
    // Example: /api/lines/{lineId}/kpi/monthly?year=2026&month=1
    [HttpGet("{lineId:guid}/kpi/monthly")]
    public async Task<IActionResult> GetMonthlyKpi(Guid lineId, [FromQuery] int year, [FromQuery] int month)
    {
        if (year < 2000 || month < 1 || month > 12)
            return BadRequest("year yoki month noto‘g‘ri");

        var from = new DateTime(year, month, 1);
        var to = from.AddMonths(1);

        var kpi = await _lineKpiService.GetLineKpiByPeriodAsync(lineId, from, to);
        return Ok(kpi);
    }
}
