using task_api.Interfaces;
using task_api.Models;
using task_api.Services;

namespace task_api.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddCors();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IIssueService, IssueService>();
            services.AddScoped<ITeamService, TeamService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.Configure<DatabaseSettings>(
                config.GetSection("TaskDatabase"));

            return services;
        }
    }
}
