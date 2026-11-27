using System.ComponentModel.DataAnnotations;
using FPPR_System.Common.Models;
using FPPR_System.DataAccess.Repositories;
using Microsoft.Extensions.Logging;

namespace FPPR_System.Business.Services;

public class LikeListService : ILikeListService
{
    public const string NotFoundMessage = "找不到這筆喜好商品，或您沒有權限操作";
    public const string SystemErrorMessage = "系統忙碌中，請稍後再試";

    private readonly ILikeListRepository _likeListRepository;
    private readonly ILogger<LikeListService> _logger;

    public LikeListService(ILikeListRepository likeListRepository, ILogger<LikeListService> logger)
    {
        _likeListRepository = likeListRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<LikeList>> GetByUserAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Array.Empty<LikeList>();

        return await _likeListRepository.GetByUserAsync(userId);
    }

    public async Task<LikeList?> GetBySNAsync(int sn, string userId)
    {
        if (sn <= 0 || string.IsNullOrWhiteSpace(userId))
            return null;

        return await _likeListRepository.GetBySNAsync(sn, userId);
    }

    public Task<ServiceResult> CreateAsync(LikeListInput input)
    {
        var validationError = Validate(input);
        if (validationError != null)
            return Task.FromResult(ServiceResult.Failure(validationError));

        return ExecuteAsync("新增", input.UserID, async () =>
        {
            await _likeListRepository.InsertAsync(input);
            return ServiceResult.Success();
        });
    }

    public Task<ServiceResult> UpdateAsync(LikeListInput input)
    {
        if (input.SN <= 0)
            return Task.FromResult(ServiceResult.Failure(NotFoundMessage));

        var validationError = Validate(input);
        if (validationError != null)
            return Task.FromResult(ServiceResult.Failure(validationError));

        return ExecuteAsync("修改", input.UserID, async () =>
        {
            var updated = await _likeListRepository.UpdateAsync(input);
            return updated ? ServiceResult.Success() : ServiceResult.Failure(NotFoundMessage);
        });
    }

    public Task<ServiceResult> DeleteAsync(int sn, string userId)
    {
        if (sn <= 0 || string.IsNullOrWhiteSpace(userId))
            return Task.FromResult(ServiceResult.Failure(NotFoundMessage));

        return ExecuteAsync("刪除", userId, async () =>
        {
            var deleted = await _likeListRepository.DeleteAsync(sn, userId);
            return deleted ? ServiceResult.Success() : ServiceResult.Failure(NotFoundMessage);
        });
    }

    private async Task<ServiceResult> ExecuteAsync(string action, string userId, Func<Task<ServiceResult>> operation)
    {
        try
        {
            return await operation();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Action}喜好清單失敗，UserID={UserID}", action, userId);
            return ServiceResult.Failure(SystemErrorMessage);
        }
    }

    private static string? Validate(LikeListInput input)
    {
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(input, new ValidationContext(input), results, validateAllProperties: true);
        return isValid ? null : string.Join("；", results.Select(r => r.ErrorMessage));
    }
}
