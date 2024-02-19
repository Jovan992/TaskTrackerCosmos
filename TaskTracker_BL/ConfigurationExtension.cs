﻿using Microsoft.Extensions.DependencyInjection;
using TaskTracker_BL.Interfaces;
using TaskTracker_BL.Services;

namespace TaskTracker_BL
{
    public static class ConfigurationExtension
    {
        public static void RegisterBusinessLogicLayer(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<IProjectService, ProjectService>();
            services.AddScoped<ITaskService, TaskService>();
        }
    }
}
