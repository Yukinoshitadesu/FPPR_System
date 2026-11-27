using System.Data;

namespace FPPR_System.DataAccess;

public interface IDbConnectionFactory
{
    IDbConnection Create();
}
