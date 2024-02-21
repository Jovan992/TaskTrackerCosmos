using TaskTracker_DAL.Interfaces;
using TaskTracker_DAL.Models;
using System.Linq.Dynamic.Core;
using CommonUtils.ResultDataResponse;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace TaskTracker_DAL.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly Container container;

    private readonly string notFoundByIdMessage = "Project with given id not found.";
    private readonly string noProjectsFoundMessage = "No projects found.";

    public ProjectRepository(IConfiguration configuration)
    {
        var databaseId = configuration["CosmosDbSettings:DatabaseName"];
        var containerId = configuration["CosmosDbSettings:ContainerId"];

        string cosmosConnectionString = GetCosmosConnectionStringFromKeyVault();

        CosmosClient cosmosClient = new(cosmosConnectionString);

        container = cosmosClient.GetDatabase(databaseId).GetContainer(containerId);
    }

    private string GetCosmosConnectionStringFromKeyVault()
    {
        SecretClientOptions options = new()
        {
            Retry =
        {
            Delay= TimeSpan.FromSeconds(2),
            MaxDelay = TimeSpan.FromSeconds(16),
            MaxRetries = 5,
            Mode = RetryMode.Exponential
         }
        };

        var client = new SecretClient(new Uri("https://jovanranisavljevkeyvault.vault.azure.net/"), new DefaultAzureCredential(), options);

        KeyVaultSecret secret = client.GetSecret("CosmosDBConnectionString");

        return secret.Value;
    }

    public async Task<ResultData<PagedList<Project>>> GetProjects(QueryStringParameters projectParameters)
    {
        var query = new QueryDefinition("SELECT * FROM Projects");

        var iterator = container.GetItemQueryIterator<Project>(query);

        var projects = new List<Project>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            projects.AddRange(response.Resource);
        }

        if (projects.Count == 0)
        {
            return new NotFoundResultData<PagedList<Project>>(noProjectsFoundMessage);
        }

        // Paging results
        PagedList<Project> result = new(
               projects,
               projects.Count,
               projectParameters.PageNumber,
               projectParameters.PageSize);

        return new OkResultData<PagedList<Project>>(result);
    }

    public async Task<ResultData<Project>> GetProjectById(int projectId)
    {
        var query = new QueryDefinition($"SELECT * FROM Projects Where Projects.ProjectId = {projectId}");

        var iterator = container.GetItemQueryIterator<Project>(query);

        var response = await iterator.ReadNextAsync();

        var project = response.Resource;

        if (!project.Any())
        {
            return new NotFoundResultData<Project>(notFoundByIdMessage);
        }

        return new OkResultData<Project>(project.First());
    }
}