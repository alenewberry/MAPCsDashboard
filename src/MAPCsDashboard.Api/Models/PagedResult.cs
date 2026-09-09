namespace MAPCsDashboard.Api.Models;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Pagina, int Tamanio, int Cantidad);

