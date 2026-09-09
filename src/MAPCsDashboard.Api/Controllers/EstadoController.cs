using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MAPCsDashboard.Api.Controllers;

[ApiController]
[Route("api/v1/estado")]
public sealed class EstadoController : ControllerBase
{
    /// <summary>Confirma que la API está disponible.</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get() => Ok(new
    {
        servicio = "API MAP Catedral Software",
        estado = "disponible",
        version = "1.0",
        fechaUtc = DateTimeOffset.UtcNow
    });
}
