using Microsoft.EntityFrameworkCore;
using Microsoft.Azure.Cosmos;
using TaskTracker_DAL;
using TaskTracker_BL;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace TaskTracker
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //using CosmosClient client = new(
            //    accountEndpoint: "https://localhost:8081/",
            //    authKeyOrResourceToken: "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");

            //Database database = await client.CreateDatabaseIfNotExistsAsync(id: "cosmicworks", throughput: 400);

            //Container container = await database.CreateContainerIfNotExistsAsync(id: "products", partitionKeyPath: "/id");

            
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddControllers().AddNewtonsoftJson();

            var connectionString = builder.Configuration.GetConnectionString("SqlConnection");
            builder.Services.RegisterDataAccessLayer(connectionString!);
            builder.Services.RegisterBusinessLogicLayer();

            builder.Services.AddSingleton<IDocumentClient>(x => new DocumentClient(new Uri(configuration["CosmosDbSettings:EndpointUri"]!), configuration["CosmosDbSettings:PrimaryKey"]));

            //builder.Services.AddSingleton((provider) =>
            //{
            //    var endpointUri = configuration["CosmosDbSettings:EndpointUri"];
            //    var primaryKey = configuration["CosmosDbSettings:PrimaryKey"];
            //    var databaseName = configuration["CosmosDbSettings:DatabaseName"];

            //    var cosmosClientOptions = new CosmosClientOptions
            //    {
            //        ApplicationName = databaseName,
            //        ConnectionMode = ConnectionMode.Gateway,

            //        //ServerCertificateCustomValidationCallback = (request, certificate, chain) =>
            //        //{
            //        //    // Always return true to ignore certificate validation errors
            //        //    return true; //not for production
            //        //}
            //    };

            //    var cosmosClient = new CosmosClient(endpointUri, primaryKey, cosmosClientOptions);

            //    return cosmosClient;
            //});

            //builder.InstallServices();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // Configure swagger authorization
            builder.Services.RegisterSwagger();

            // Configure JWT authentication
            builder.Services.RegisterAuthentication(builder);

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
