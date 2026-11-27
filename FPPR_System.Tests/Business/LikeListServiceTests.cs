using FPPR_System.Business.Services;
using FPPR_System.Common.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace FPPR_System.Tests.Business;

public class LikeListServiceTests
{
    private readonly FakeLikeListRepository _repository = new();
    private readonly LikeListService _service;

    public LikeListServiceTests()
    {
        _service = new LikeListService(_repository, NullLogger<LikeListService>.Instance);
    }

    private static LikeListInput CreateValidInput() => new()
    {
        SN = 1,
        UserID = "A1236456789",
        ProductName = "0050",
        Price = 150m,
        FeeRate = 0.01m,
        PurchaseQuantity = 10,
        Account = "1111999666"
    };

    [Fact]
    public async Task CreateAsync_ReturnsSuccess_WhenInputIsValid()
    {
        var result = await _service.CreateAsync(CreateValidInput());

        Assert.True(result.IsSuccess);
        Assert.Equal(1, _repository.CallCount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CreateAsync_ReturnsFailureWithoutCallingRepository_WhenQuantityIsNotPositive(int quantity)
    {
        var input = CreateValidInput();
        input.PurchaseQuantity = quantity;

        var result = await _service.CreateAsync(input);

        Assert.False(result.IsSuccess);
        Assert.Contains("購買數量", result.ErrorMessage);
        Assert.Equal(0, _repository.CallCount);
    }

    [Theory]
    [InlineData("")]
    [InlineData("12ab34")]
    [InlineData("123456789012345678901")]
    public async Task CreateAsync_ReturnsFailure_WhenAccountIsInvalid(string account)
    {
        var input = CreateValidInput();
        input.Account = account;

        var result = await _service.CreateAsync(input);

        Assert.False(result.IsSuccess);
        Assert.Equal(0, _repository.CallCount);
    }

    [Fact]
    public async Task CreateAsync_ReturnsAllErrors_WhenMultipleFieldsAreInvalid()
    {
        var input = CreateValidInput();
        input.ProductName = "";
        input.FeeRate = 1.5m;

        var result = await _service.CreateAsync(input);

        Assert.Contains("產品名稱", result.ErrorMessage);
        Assert.Contains("手續費率", result.ErrorMessage);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFriendlyMessage_WhenRepositoryThrows()
    {
        _repository.ExceptionToThrow = new InvalidOperationException("Login failed for user 'sa'");

        var result = await _service.CreateAsync(CreateValidInput());

        Assert.False(result.IsSuccess);
        Assert.Equal(LikeListService.SystemErrorMessage, result.ErrorMessage);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNotFound_WhenRepositoryUpdatesNothing()
    {
        _repository.UpdateResult = false;

        var result = await _service.UpdateAsync(CreateValidInput());

        Assert.False(result.IsSuccess);
        Assert.Equal(LikeListService.NotFoundMessage, result.ErrorMessage);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNotFoundWithoutCallingRepository_WhenSNIsInvalid()
    {
        var input = CreateValidInput();
        input.SN = 0;

        var result = await _service.UpdateAsync(input);

        Assert.False(result.IsSuccess);
        Assert.Equal(0, _repository.CallCount);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsSuccess_WhenRepositoryDeletes()
    {
        var result = await _service.DeleteAsync(1, "A1236456789");

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNotFound_WhenRepositoryDeletesNothing()
    {
        _repository.DeleteResult = false;

        var result = await _service.DeleteAsync(1, "A1236456789");

        Assert.Equal(LikeListService.NotFoundMessage, result.ErrorMessage);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNotFoundWithoutCallingRepository_WhenUserIdIsEmpty()
    {
        var result = await _service.DeleteAsync(1, "");

        Assert.False(result.IsSuccess);
        Assert.Equal(0, _repository.CallCount);
    }

    [Fact]
    public async Task GetByUserAsync_ReturnsEmptyWithoutCallingRepository_WhenUserIdIsEmpty()
    {
        var likeLists = await _service.GetByUserAsync(" ");

        Assert.Empty(likeLists);
        Assert.Equal(0, _repository.CallCount);
    }
}
