using FPPR_System.Common.Models;

namespace FPPR_System.DataAccess.Repositories;

public interface ILikeListRepository
{
    Task<IReadOnlyList<LikeList>> GetByUserAsync(string userId);
    Task<LikeList?> GetBySNAsync(int sn, string userId);
    Task<int> InsertAsync(LikeListInput input);
    Task<bool> UpdateAsync(LikeListInput input);
    Task<bool> DeleteAsync(int sn, string userId);
}
