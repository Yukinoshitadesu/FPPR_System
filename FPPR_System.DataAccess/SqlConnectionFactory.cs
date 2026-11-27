using System.Data;
using Microsoft.Data.SqlClient;

namespace FPPR_System.DataAccess;

public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("資料庫連線字串未設定", nameof(connectionString));

        _connectionString = connectionString;
    }

    public IDbConnection Create() => new SqlConnection(_connectionString);
}
