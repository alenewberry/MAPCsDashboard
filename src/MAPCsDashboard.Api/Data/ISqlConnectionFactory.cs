using Microsoft.Data.SqlClient;

namespace MAPCsDashboard.Api.Data;

public interface ISqlConnectionFactory
{
    SqlConnection Create();
}

