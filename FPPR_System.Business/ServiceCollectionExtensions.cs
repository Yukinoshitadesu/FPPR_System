using FPPR_System.Business.Services;
using FPPR_System.DataAccess;
using FPPR_System.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace FPPR_System.Business;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(new SqlConnectionFactory(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ILikeListRepository, LikeListRepository>();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ILikeListService, LikeListService>();

        return services;
    }
}
