using Cosmonaut;
using Cosmonaut.Extensions.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using TaskTracker_DAL.Models;

namespace TaskTracker_DAL;

public static class CosmosInstaller
{
    public static void InstallServices(this WebApplicationBuilder builder)
    {


        var cosmosStoreSettings = new CosmosStoreSettings(
            builder.Configuration["CosmosSettings:DatabaseName"],
            builder.Configuration["CosmosSettings:AccountUri"],
            builder.Configuration["CosmosSettings:AccountKey"],
            new ConnectionPolicy { ConnectionMode = ConnectionMode.Direct, ConnectionProtocol = Protocol.Tcp });

        builder.Services.AddCosmosStore<Project>(cosmosStoreSettings);

    }
}
