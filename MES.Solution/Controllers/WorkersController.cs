using MES.Application.DTOs;
using MES.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MES.Solution.Controllers;

[ApiController]
[Route("api/workers")]
public class WorkersController : ControllerBase
{
    private readonly WorkerService _service;

    public WorkersController(WorkerService service)
    {
        _service = service;
    }

    // GET /api/workers?isActive=true
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool? isActive = null)
    {
        var list = await _service.GetAllAsync(isActive);
        return Ok(list);
    }

    // GET /api/workers/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var worker = await _service.GetByIdAsync(id);
        if (worker == null) return NotFound();
        return Ok(worker);
    }

    // POST /api/workers
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WorkerCreateDto dto)
    {
        try
        {
            var id = await _service.CreateAsync(dto);
            return Ok(new { id });
        }
        catch (Exception ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    // PUT /api/workers/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] WorkerUpdateDto dto)
    {
        try
        {
            await _service.UpdateAsync(id, dto);
            return Ok(new { message = "UPDATED" });
        }
        catch (Exception ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    // DELETE /api/workers/{id}  (soft delete)
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        try
        {
            await _service.DeactivateAsync(id);
            return Ok(new { message = "DEACTIVATED" });
        }
        catch (Exception ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }
}
