using CommonUtils.DateTime;
using Microsoft.OpenApi.Models;

namespace TaskTracker;

public static class ConfigurationExtension
{
    public static void RegisterSwaggerInfoAndSchemaFilter(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "TaskTrackerCosmos",
                Version = "v1"
            });

            c.SchemaFilter<DateOnlySchemaFilter>();
        });
    }
}
