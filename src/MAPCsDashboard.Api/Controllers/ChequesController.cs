using MAPCsDashboard.Api.Authentication;
using MAPCsDashboard.Api.Data;
using MAPCsDashboard.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MAPCsDashboard.Api.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = ApiKeyAuthenticationDefaults.Scheme)]
[Route("api/v1/cheques")]
public sealed class ChequesController(IChequeRepository repository) : ControllerBase
{
    /// <summary>Lista cheques de terceros con filtros opcionales.</summary>
    /// <remarks>
    /// Ejemplo: /api/v1/cheques?estado=En%20cartera&amp;empresa=30-12345678-9&amp;fechaCobroDesde=2026-07-01
    /// </remarks>
    [HttpGet]
    [ProducesResponseType<PagedResult<ChequeDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<ChequeDto>>> List(
        [FromQuery] ChequeQuery query,
        CancellationToken cancellationToken)
    {
        if (query.Pagina < 1 || query.Tamanio is < 1 or > 200)
        {
            ModelState.AddModelError(nameof(query.Pagina), "Página debe ser mayor o igual a 1.");
            ModelState.AddModelError(nameof(query.Tamanio), "Tamaño debe estar entre 1 y 200.");
            return ValidationProblem(ModelState);
        }

        if (query.FechaCobroDesde > query.FechaCobroHasta)
        {
            ModelState.AddModelError(nameof(query.FechaCobroDesde), "La fecha desde no puede ser posterior a la fecha hasta.");
            return ValidationProblem(ModelState);
        }

        return Ok(await repository.ListAsync(query, cancellationToken));
    }

    /// <summary>Obtiene el detalle de un cheque por su identificador.</summary>
    [HttpGet("{id}")]
    [ProducesResponseType<ChequeDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChequeDto>> GetById(string id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id) || id.Length > 15)
        {
            return BadRequest(new ProblemDetails { Title = "Identificador de cheque inválido." });
        }

        var cheque = await repository.GetByIdAsync(id, cancellationToken);
        return cheque is null ? NotFound() : Ok(cheque);
    }

    /// <summary>Lista el ingreso y todos los cambios de estado de un cheque.</summary>
    [HttpGet("{id}/movimientos")]
    [ProducesResponseType<IReadOnlyList<ChequeMovimientoDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<ChequeMovimientoDto>>> ListMovements(
        string id,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id) || id.Length > 15)
        {
            return BadRequest(new ProblemDetails { Title = "Identificador de cheque inválido." });
        }

        return Ok(await repository.ListMovementsAsync(id, cancellationToken));
    }

    /// <summary>Lista los estados disponibles para usar como filtro.</summary>
    [HttpGet("estados")]
    [ProducesResponseType<IReadOnlyList<string>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<string>>> ListStates(CancellationToken cancellationToken) =>
        Ok(await repository.ListStatesAsync(cancellationToken));
}
