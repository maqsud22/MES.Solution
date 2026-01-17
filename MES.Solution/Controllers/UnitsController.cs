using Microsoft.AspNetCore.Mvc;
using MES.Application.Services;

[ApiController]
[Route("api/units")]
public class UnitsController : ControllerBase
{
    private readonly UnitTraceService _traceService;
    private readonly UnitQueryService _unitQueryService;

    public UnitsController(UnitTraceService traceService, UnitQueryService unitQueryService)
    {
        _traceService = traceService;
        _unitQueryService = unitQueryService;
    }

    // ✅ Trace by serial
    [HttpGet("{serialNumber}")]
    public async Task<IActionResult> Get(string serialNumber)
    {
        var dto = await _traceService.GetBySerialAsync(serialNumber);
        if (dto == null)
            return NotFound(new { error = "Serial topilmadi" });

        return Ok(dto);
    }

    // ✅ Recent units by line
    [HttpGet("lines/{lineId}/recent")]
    public async Task<IActionResult> Recent(Guid lineId, [FromQuery] int take = 50)
    {
        var data = await _unitQueryService.GetRecentByLineAsync(lineId, take);
        return Ok(data);
    }
}
