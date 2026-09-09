namespace MAPCsDashboard.Api.Models;

public sealed record ChequeDto(
    string Id,
    string Numero,
    string? Banco,
    decimal Importe,
    string Moneda,
    DateOnly? FechaEmision,
    DateOnly FechaCobro,
    DateTime? FechaIngresoCartera,
    DateTime? FechaSalidaCartera,
    string Estado,
    string? EmpresaOrigen,
    string? CuitEmpresaOrigen);

