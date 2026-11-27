using System.Data;
using Dapper;
using FPPR_System.Common.Models;

namespace FPPR_System.DataAccess.Repositories;

public class LikeListRepository : ILikeListRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public LikeListRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<LikeList>> GetByUserAsync(string userId)
    {
        using var connection = _connectionFactory.Create();
        var likeLists = await connection.QueryAsync<LikeList>(
            "dbo.usp_LikeList_GetByUser",
            new { UserID = userId },
            commandType: CommandType.StoredProcedure);
        return likeLists.ToList();
    }

    public async Task<LikeList?> GetBySNAsync(int sn, string userId)
    {
        using var connection = _connectionFactory.Create();
        return await connection.QuerySingleOrDefaultAsync<LikeList>(
            "dbo.usp_LikeList_GetBySN",
            new { SN = sn, UserID = userId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> InsertAsync(LikeListInput input)
    {
        using var connection = _connectionFactory.Create();
        return await connection.ExecuteScalarAsync<int>(
            "dbo.usp_LikeList_Insert",
            new
            {
                input.UserID,
                input.ProductName,
                input.Price,
                input.FeeRate,
                input.PurchaseQuantity,
                input.Account
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> UpdateAsync(LikeListInput input)
    {
        using var connection = _connectionFactory.Create();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            "dbo.usp_LikeList_Update",
            new
            {
                input.SN,
                input.UserID,
                input.ProductName,
                input.Price,
                input.FeeRate,
                input.PurchaseQuantity,
                input.Account
            },
            commandType: CommandType.StoredProcedure);
        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(int sn, string userId)
    {
        using var connection = _connectionFactory.Create();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            "dbo.usp_LikeList_Delete",
            new { SN = sn, UserID = userId },
            commandType: CommandType.StoredProcedure);
        return affectedRows > 0;
    }
}
