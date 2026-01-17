using Microsoft.AspNetCore.Mvc;
using MES.Application.Services;

namespace MES.Solution.Controllers;

[ApiController]
[Route("api/workorders")]
public class WorkOrdersController : ControllerBase
{
    private readonly WorkOrderQueryService _service;
    public WorkOrdersController(WorkOrderQueryService service) => _service = service;

    [HttpGet("lines/{lineId}/active")]
    public async Task<IActionResult> GetActive(Guid lineId)
        => Ok(await _service.GetActiveByLineAsync(lineId));
}
