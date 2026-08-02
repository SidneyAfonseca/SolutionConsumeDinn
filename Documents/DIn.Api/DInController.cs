
using Microsoft.AspNetCore.Mvc;
using DIn.Service;
using DIn.Domain;

namespace DIn.Api.Controllers;

[ApiController]
[Route("api/din")]
public class DInController : ControllerBase
{
    private readonly IDInService _service;

    public DInController(IDInService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Enviar([FromBody] List<DInRequest> request)
    {
        var result = await _service.EnviarAsync(request);
        return Ok(result);
    }
}
