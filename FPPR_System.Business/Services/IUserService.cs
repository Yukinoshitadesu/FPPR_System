using FPPR_System.Common.Models;

namespace FPPR_System.Business.Services;

public interface IUserService
{
    Task<IReadOnlyList<User>> GetAllAsync();
}
