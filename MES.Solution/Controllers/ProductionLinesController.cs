using Microsoft.AspNetCore.Mvc;
using MES.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MES.Solution.Controllers;

[ApiController]
[Route("api/production-lines")]
public class ProductionLinesController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public ProductionLinesController(IApplicationDbContext context)
    {
        _context = context;
    }

    // ✅ Line ro‘yxati (Id, Code, Name, Status)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var lines = await _context.ProductionLines
            .OrderBy(x => x.Code)
            .Select(x => new
            {
                x.Id,
                x.Code,
                x.Name,
                x.Status
            })
            .ToListAsync();

        return Ok(lines);
    }
}
