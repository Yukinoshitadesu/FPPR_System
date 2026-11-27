using FPPR_System.Common.Models;

namespace FPPR_System.Business.Services;

public interface ILikeListService
{
    Task<IReadOnlyList<LikeList>> GetByUserAsync(string userId);
    Task<LikeList?> GetBySNAsync(int sn, string userId);
    Task<ServiceResult> CreateAsync(LikeListInput input);
    Task<ServiceResult> UpdateAsync(LikeListInput input);
    Task<ServiceResult> DeleteAsync(int sn, string userId);
}
