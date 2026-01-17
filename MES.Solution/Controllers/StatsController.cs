using Microsoft.AspNetCore.Mvc;
using MES.Application.Services;

namespace MES.Solution.Controllers;

[ApiController]
[Route("api/stats")]
public class StatsController : ControllerBase
{
    private readonly OperatorStatsService _service;

    public StatsController(OperatorStatsService service)
    {
        _service = service;
    }

    // ✅ Front uchun: hamma operatorlar kunlik statistikasi
    // GET /api/stats/operators/daily?day=2026-01-12&lineId=...
    [HttpGet("operators/daily")]
    public async Task<IActionResult> OperatorsDaily([FromQuery] DateTime? day = null, [FromQuery] Guid? lineId = null)
    {
        var d = day ?? DateTime.UtcNow;
        var data = await _service.GetDailyAllAsync(d, lineId);
        return Ok(data);
    }

    // ✅ Bitta operator uchun
    // GET /api/stats/operators/{operatorId}/daily?day=2026-01-12
    [HttpGet("operators/{operatorId}/daily")]
    public async Task<IActionResult> OperatorDaily(Guid operatorId, [FromQuery] DateTime? day = null)
    {
        var d = day ?? DateTime.UtcNow;
        var dto = await _service.GetDailyAsync(operatorId, d);
        return Ok(dto);
    }
}
