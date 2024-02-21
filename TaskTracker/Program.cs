using TaskTracker_DAL;
using TaskTracker_BL;

namespace TaskTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddControllers().AddNewtonsoftJson();

            builder.Services.RegisterDataAccessLayer();
            builder.Services.RegisterBusinessLogicLayer();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // Configure swagger datetime schema
            builder.Services.RegisterSwagger();

            var app = builder.Build();

            app.UseHttpsRedirection();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapControllers();

            app.Run();
        }
    }
}
