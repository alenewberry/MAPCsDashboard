using MAPCsDashboard.Api.Models;

namespace MAPCsDashboard.Api.Data;

public interface IChequeRepository
{
    Task<PagedResult<ChequeDto>> ListAsync(ChequeQuery query, CancellationToken cancellationToken);
    Task<ChequeDto?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyList<ChequeMovimientoDto>> ListMovementsAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyList<string>> ListStatesAsync(CancellationToken cancellationToken);
}
