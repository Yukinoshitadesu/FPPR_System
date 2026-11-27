using Dapper;
using FPPR_System.Common.Models;
using FPPR_System.DataAccess;
using FPPR_System.DataAccess.Repositories;
using Xunit;

namespace FPPR_System.Tests.DataAccess;

public class LikeListRepositoryTests
{
    private readonly IDbConnectionFactory _connectionFactory = TestDatabase.CreateConnectionFactory();
    private readonly LikeListRepository _likeListRepository;

    public LikeListRepositoryTests()
    {
        _likeListRepository = new LikeListRepository(_connectionFactory);
    }

    private static LikeListInput CreateInput(string userId = TestDatabase.UserA) => new()
    {
        UserID = userId,
        ProductName = "測試商品",
        Price = 100m,
        FeeRate = 0.01m,
        PurchaseQuantity = 2,
        Account = "1111999666"
    };

    [Fact]
    public async Task InsertAsync_CalculatesTotalFeeAndTotalAmount()
    {
        using var scope = TestDatabase.BeginRollbackScope();

        var sn = await _likeListRepository.InsertAsync(CreateInput());
        var likeList = await _likeListRepository.GetBySNAsync(sn, TestDatabase.UserA);

        Assert.NotNull(likeList);
        Assert.Equal(2.00m, likeList!.TotalFee);
        Assert.Equal(202.00m, likeList.TotalAmount);
        Assert.Equal("test@email.com", likeList.Email);
    }

    [Fact]
    public async Task InsertAsync_StoresSqlInjectionTextAsPlainText()
    {
        using var scope = TestDatabase.BeginRollbackScope();
        var input = CreateInput();
        input.ProductName = "x'; DROP TABLE dbo.Users;--";

        var sn = await _likeListRepository.InsertAsync(input);
        var likeList = await _likeListRepository.GetBySNAsync(sn, TestDatabase.UserA);

        Assert.Equal(input.ProductName, likeList!.ProductName);
    }

    [Fact]
    public async Task InsertAsync_RollsBackProduct_WhenUserDoesNotExist()
    {
        var productCountBefore = await CountProductsAsync();

        await Assert.ThrowsAnyAsync<Exception>(() => _likeListRepository.InsertAsync(CreateInput("NOT_EXIST")));

        Assert.Equal(productCountBefore, await CountProductsAsync());
    }

    [Fact]
    public async Task GetByUserAsync_ReturnsOnlyThatUsersData()
    {
        using var scope = TestDatabase.BeginRollbackScope();
        await _likeListRepository.InsertAsync(CreateInput(TestDatabase.UserB));

        var likeLists = await _likeListRepository.GetByUserAsync(TestDatabase.UserA);

        Assert.NotEmpty(likeLists);
        Assert.All(likeLists, l => Assert.Equal("test@email.com", l.Email));
    }

    [Fact]
    public async Task GetBySNAsync_ReturnsNull_WhenQueryingOtherUsersData()
    {
        using var scope = TestDatabase.BeginRollbackScope();
        var sn = await _likeListRepository.InsertAsync(CreateInput());

        var likeList = await _likeListRepository.GetBySNAsync(sn, TestDatabase.UserB);

        Assert.Null(likeList);
    }

    [Fact]
    public async Task UpdateAsync_RecalculatesTotals_WhenQuantityChanges()
    {
        using var scope = TestDatabase.BeginRollbackScope();
        var input = CreateInput();
        input.SN = await _likeListRepository.InsertAsync(input);
        input.PurchaseQuantity = 5;

        var updated = await _likeListRepository.UpdateAsync(input);
        var likeList = await _likeListRepository.GetBySNAsync(input.SN, TestDatabase.UserA);

        Assert.True(updated);
        Assert.Equal(5.00m, likeList!.TotalFee);
        Assert.Equal(505.00m, likeList.TotalAmount);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenUpdatingOtherUsersData()
    {
        using var scope = TestDatabase.BeginRollbackScope();
        var input = CreateInput();
        input.SN = await _likeListRepository.InsertAsync(input);
        input.UserID = TestDatabase.UserB;
        input.ProductName = "被竄改";

        var updated = await _likeListRepository.UpdateAsync(input);
        var likeList = await _likeListRepository.GetBySNAsync(input.SN, TestDatabase.UserA);

        Assert.False(updated);
        Assert.Equal("測試商品", likeList!.ProductName);
    }

    [Fact]
    public async Task DeleteAsync_RemovesLikeListAndProduct_WhenOwnerDeletes()
    {
        using var scope = TestDatabase.BeginRollbackScope();
        var sn = await _likeListRepository.InsertAsync(CreateInput());
        var productNo = (await _likeListRepository.GetBySNAsync(sn, TestDatabase.UserA))!.ProductNo;

        var deleted = await _likeListRepository.DeleteAsync(sn, TestDatabase.UserA);

        Assert.True(deleted);
        Assert.Null(await _likeListRepository.GetBySNAsync(sn, TestDatabase.UserA));
        Assert.False(await ProductExistsAsync(productNo));
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenDeletingOtherUsersData()
    {
        using var scope = TestDatabase.BeginRollbackScope();
        var sn = await _likeListRepository.InsertAsync(CreateInput());

        var deleted = await _likeListRepository.DeleteAsync(sn, TestDatabase.UserB);

        Assert.False(deleted);
        Assert.NotNull(await _likeListRepository.GetBySNAsync(sn, TestDatabase.UserA));
    }

    private async Task<int> CountProductsAsync()
    {
        using var connection = _connectionFactory.Create();
        return await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM dbo.Product");
    }

    private async Task<bool> ProductExistsAsync(int productNo)
    {
        using var connection = _connectionFactory.Create();
        return await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM dbo.Product WHERE No = @productNo", new { productNo }) > 0;
    }
}
