using Microsoft.Extensions.DependencyInjection;
using TaskTracker_DAL.Interfaces;
using TaskTracker_DAL.Repositories;

namespace TaskTracker_DAL
{
    public static class ConfigurationExtension
    {
        public static void RegisterDataAccessLayer(this IServiceCollection services)
        {
            services.AddSingleton<IProjectRepository, ProjectRepository>();
        }
    }
}