using System.Data;
using MAPCsDashboard.Api.Models;
using Microsoft.Data.SqlClient;

namespace MAPCsDashboard.Api.Data;

public sealed class SqlChequeRepository(ISqlConnectionFactory connectionFactory) : IChequeRepository
{
    private const string Columns = """
        Id, Numero, Banco, Importe, Moneda, FechaEmision, FechaCobro,
        FechaIngresoCartera, FechaSalidaCartera, Estado, EmpresaOrigen, CuitEmpresaOrigen
        """;

    public async Task<PagedResult<ChequeDto>> ListAsync(ChequeQuery query, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, Numero, Banco, Importe, Moneda, FechaEmision, FechaCobro,
                   FechaIngresoCartera, FechaSalidaCartera, Estado, EmpresaOrigen, CuitEmpresaOrigen
            FROM api.vw_ChequesTerceros
            WHERE (@Estado IS NULL OR Estado = @Estado)
              AND (@Empresa IS NULL OR EmpresaOrigen = @Empresa OR CuitEmpresaOrigen = @Empresa)
              AND (@Desde IS NULL OR FechaCobro >= @Desde)
              AND (@Hasta IS NULL OR FechaCobro <= @Hasta)
            ORDER BY FechaCobro, Id
            OFFSET @Offset ROWS FETCH NEXT @Tamanio ROWS ONLY;
            """;

        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection) { CommandTimeout = 30 };
        AddNullableString(command, "@Estado", query.Estado);
        AddNullableString(command, "@Empresa", query.Empresa);
        AddNullableDate(command, "@Desde", query.FechaCobroDesde);
        AddNullableDate(command, "@Hasta", query.FechaCobroHasta);
        command.Parameters.Add("@Offset", SqlDbType.Int).Value = (query.Pagina - 1) * query.Tamanio;
        command.Parameters.Add("@Tamanio", SqlDbType.Int).Value = query.Tamanio;

        var items = new List<ChequeDto>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            items.Add(ReadCheque(reader));
        }

        return new PagedResult<ChequeDto>(items, query.Pagina, query.Tamanio, items.Count);
    }

    public async Task<ChequeDto?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var sql = $"SELECT {Columns} FROM api.vw_ChequesTerceros WHERE Id = @Id;";
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection) { CommandTimeout = 30 };
        command.Parameters.Add("@Id", SqlDbType.NVarChar, 15).Value = id;
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? ReadCheque(reader) : null;
    }

    public async Task<IReadOnlyList<ChequeMovimientoDto>> ListMovementsAsync(
        string id,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Estado, Fecha, Comprobante, Detalle
            FROM api.vw_ChequesTercerosMovimientos
            WHERE ChequeId = @Id
            ORDER BY Fecha, Orden;
            """;

        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection) { CommandTimeout = 30 };
        command.Parameters.Add("@Id", SqlDbType.NVarChar, 15).Value = id;
        var items = new List<ChequeMovimientoDto>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            items.Add(new ChequeMovimientoDto(
                reader.GetString(reader.GetOrdinal("Estado")),
                reader.GetDateTime(reader.GetOrdinal("Fecha")),
                GetNullableString(reader, "Comprobante"),
                GetNullableString(reader, "Detalle")));
        }

        return items;
    }

    public async Task<IReadOnlyList<string>> ListStatesAsync(CancellationToken cancellationToken)
    {
        const string sql = "SELECT DISTINCT Estado FROM api.vw_ChequesTerceros WHERE Estado IS NOT NULL ORDER BY Estado;";
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection) { CommandTimeout = 30 };
        var states = new List<string>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            states.Add(reader.GetString(0));
        }

        return states;
    }

    private static ChequeDto ReadCheque(SqlDataReader reader) => new(
        reader.GetString(reader.GetOrdinal("Id")),
        reader.GetString(reader.GetOrdinal("Numero")),
        GetNullableString(reader, "Banco"),
        reader.GetDecimal(reader.GetOrdinal("Importe")),
        reader.GetString(reader.GetOrdinal("Moneda")),
        GetNullableDateOnly(reader, "FechaEmision"),
        DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("FechaCobro"))),
        GetNullableDateTime(reader, "FechaIngresoCartera"),
        GetNullableDateTime(reader, "FechaSalidaCartera"),
        reader.GetString(reader.GetOrdinal("Estado")),
        GetNullableString(reader, "EmpresaOrigen"),
        GetNullableString(reader, "CuitEmpresaOrigen"));

    private static string? GetNullableString(SqlDataReader reader, string name)
    {
        var ordinal = reader.GetOrdinal(name);
        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }

    private static DateTime? GetNullableDateTime(SqlDataReader reader, string name)
    {
        var ordinal = reader.GetOrdinal(name);
        return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
    }

    private static DateOnly? GetNullableDateOnly(SqlDataReader reader, string name)
    {
        var value = GetNullableDateTime(reader, name);
        return value.HasValue ? DateOnly.FromDateTime(value.Value) : null;
    }

    private static void AddNullableString(SqlCommand command, string name, string? value) =>
        command.Parameters.Add(name, SqlDbType.NVarChar, 200).Value =
            string.IsNullOrWhiteSpace(value) ? DBNull.Value : value.Trim();

    private static void AddNullableDate(SqlCommand command, string name, DateOnly? value) =>
        command.Parameters.Add(name, SqlDbType.Date).Value =
            value.HasValue ? value.Value.ToDateTime(TimeOnly.MinValue) : DBNull.Value;
}
