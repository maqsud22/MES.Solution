using Microsoft.AspNetCore.Mvc;
using MES.Application.DTOs;
using MES.Application.Services;

namespace MES.Solution.Controllers;

[ApiController]
[Route("api/scan")]
public class ScanController : ControllerBase
{
    private readonly ScanService _scanService;

    public ScanController(ScanService scanService)
    {
        _scanService = scanService;
    }

    [HttpPost]
    public async Task<IActionResult> Scan([FromBody] ScanRequestDto dto)
    {
        try
        {
            // ✅ ScanService hozir Task (void) qaytarayotgan bo‘lsa ham xatosiz ishlaydi
            await _scanService.ScanAsync(dto);

            return Ok(new { message = "SCAN OK" });
        }
        catch (Exception ex)
        {
            // ✅ Biznes qoidalar buzilganda 500 emas, 409 qaytaramiz
            return Conflict(new { error = ex.Message });
        }
    }
}
