using FPPR_System.Common.Models;

namespace FPPR_System.DataAccess.Repositories;

public interface IUserRepository
{
    Task<IReadOnlyList<User>> GetAllAsync();
}
