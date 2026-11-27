using FPPR_System.Common.Models;
using FPPR_System.DataAccess.Repositories;

namespace FPPR_System.Tests.Business;

public class FakeLikeListRepository : ILikeListRepository
{
    public bool UpdateResult { get; set; } = true;
    public bool DeleteResult { get; set; } = true;
    public Exception? ExceptionToThrow { get; set; }
    public int CallCount { get; private set; }

    public Task<IReadOnlyList<LikeList>> GetByUserAsync(string userId)
    {
        CallCount++;
        ThrowIfConfigured();
        return Task.FromResult<IReadOnlyList<LikeList>>(new List<LikeList> { new() { SN = 1 } });
    }

    public Task<LikeList?> GetBySNAsync(int sn, string userId)
    {
        CallCount++;
        ThrowIfConfigured();
        return Task.FromResult<LikeList?>(new LikeList { SN = sn });
    }

    public Task<int> InsertAsync(LikeListInput input)
    {
        CallCount++;
        ThrowIfConfigured();
        return Task.FromResult(1);
    }

    public Task<bool> UpdateAsync(LikeListInput input)
    {
        CallCount++;
        ThrowIfConfigured();
        return Task.FromResult(UpdateResult);
    }

    public Task<bool> DeleteAsync(int sn, string userId)
    {
        CallCount++;
        ThrowIfConfigured();
        return Task.FromResult(DeleteResult);
    }

    private void ThrowIfConfigured()
    {
        if (ExceptionToThrow != null)
            throw ExceptionToThrow;
    }
}
