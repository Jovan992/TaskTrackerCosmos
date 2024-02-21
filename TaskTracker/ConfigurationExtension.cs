using CommonUtils.DateTime;

namespace TaskTracker
{
    public static class ConfigurationExtension
    {
        public static void RegisterSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SchemaFilter<DateOnlySchemaFilter>();
            });
        }
    }
}
