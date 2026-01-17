using Microsoft.AspNetCore.Mvc;
using MES.Application.Services;

namespace MES.Solution.Controllers;

[ApiController]
[Route("api/lines/overview")]
public class LineOverviewController : ControllerBase
{
    private readonly LineOverviewService _service;

    public LineOverviewController(LineOverviewService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var data = await _service.GetAllAsync();
        return Ok(data);
    }
}
