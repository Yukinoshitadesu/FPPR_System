using System.Transactions;
using FPPR_System.DataAccess;

namespace FPPR_System.Tests;
// 對應seeddata.sql的資料
public static class TestDatabase
{
    public const string UserA = "A1236456789";
    public const string UserB = "B2345678901";

    //組出測試用資料庫連線
    public static IDbConnectionFactory CreateConnectionFactory() =>
        new SqlConnectionFactory(GetConnectionString());
    //建立回滾交易範圍
    public static TransactionScope BeginRollbackScope() =>
        new(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

    private static string GetConnectionString()
    {
        var fromEnvironment = Environment.GetEnvironmentVariable("FPPR_TEST_CONNECTION");
        if (!string.IsNullOrWhiteSpace(fromEnvironment))
            return fromEnvironment;

        var password = ReadSaPasswordFromEnvFile();
        return $"Server=localhost,1433;Database=FPPR;User Id=sa;Password={password};TrustServerCertificate=True";
    }

    private static string ReadSaPasswordFromEnvFile()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null && !File.Exists(Path.Combine(directory.FullName, ".env")))
            directory = directory.Parent;

        if (directory == null)
            throw new InvalidOperationException("找不到 .env，請確認從專案目錄執行測試");

        var line = File.ReadAllLines(Path.Combine(directory.FullName, ".env"))
            .FirstOrDefault(l => l.StartsWith("MSSQL_SA_PASSWORD="));

        return line?.Split('=', 2)[1]
            ?? throw new InvalidOperationException(".env 缺少 MSSQL_SA_PASSWORD");
    }
}
