using System.Data;
using Dapper;
using FPPR_System.Common.Models;

namespace FPPR_System.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<User>> GetAllAsync()
    {
        using var connection = _connectionFactory.Create();
        var users = await connection.QueryAsync<User>(
            "dbo.usp_User_GetAll",
            commandType: CommandType.StoredProcedure);
        return users.ToList();
    }
}
