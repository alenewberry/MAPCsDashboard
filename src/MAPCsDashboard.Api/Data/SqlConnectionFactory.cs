using Microsoft.Data.SqlClient;

namespace MAPCsDashboard.Api.Data;

public sealed class SqlConnectionFactory(IConfiguration configuration) : ISqlConnectionFactory
{
    public SqlConnection Create()
    {
        var connectionString = configuration.GetConnectionString("Erp");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Falta ConnectionStrings:Erp.");
        }

        return new SqlConnection(connectionString);
    }
}

