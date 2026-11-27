using FPPR_System.DataAccess.Repositories;
using Xunit;

namespace FPPR_System.Tests.DataAccess;

public class UserRepositoryTests
{
    private readonly UserRepository _userRepository = new(TestDatabase.CreateConnectionFactory());

    [Fact]
    public async Task GetAllAsync_ReturnsSeedUsers()
    {
        var users = await _userRepository.GetAllAsync();

        Assert.Contains(users, u => u.UserID == TestDatabase.UserA && u.Email == "test@email.com");
        Assert.Contains(users, u => u.UserID == TestDatabase.UserB);
    }
}
