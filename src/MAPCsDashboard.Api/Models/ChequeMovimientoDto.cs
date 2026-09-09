namespace MAPCsDashboard.Api.Models;

public sealed record ChequeMovimientoDto(
    string Estado,
    DateTime Fecha,
    string? Comprobante,
    string? Detalle);
