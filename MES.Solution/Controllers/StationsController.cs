using Microsoft.AspNetCore.Mvc;
using MES.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MES.Solution.Controllers;

[ApiController]
[Route("api/lines/{lineId}/stations")]
public class StationsController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public StationsController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetByLine(Guid lineId)
    {
        var stations = await _context.Stations
            .Where(x => x.ProductionLineId == lineId)
            .OrderBy(x => x.Sequence)
            .Select(x => new
            {
                x.Id,
                x.Code,
                x.Name,
                x.Sequence,
                x.Type
            })
            .ToListAsync();

        return Ok(stations);
    }
}
