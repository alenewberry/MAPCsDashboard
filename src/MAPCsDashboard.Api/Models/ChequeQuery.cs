namespace MAPCsDashboard.Api.Models;

public sealed class ChequeQuery
{
    public string? Estado { get; init; }
    public string? Empresa { get; init; }
    public DateOnly? FechaCobroDesde { get; init; }
    public DateOnly? FechaCobroHasta { get; init; }
    public int Pagina { get; init; } = 1;
    public int Tamanio { get; init; } = 50;
}

