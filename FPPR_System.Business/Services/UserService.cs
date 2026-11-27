using FPPR_System.Common.Models;
using FPPR_System.DataAccess.Repositories;

namespace FPPR_System.Business.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<IReadOnlyList<User>> GetAllAsync() => _userRepository.GetAllAsync();
}
