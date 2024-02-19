using TaskTracker_DAL.Interfaces;
using TaskTracker_DAL.Models;
using System.Linq.Dynamic.Core;
using CommonUtils.ResultDataResponse;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace TaskTracker_DAL.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly IRepositoryBase<Project> repositoryBase;
    private readonly IConfiguration configuration;
    private readonly IDocumentClient documentClient;
    readonly string databaseId;
    readonly string collectionId;

    private readonly string notFoundByIdMessage = "Project with given id not found.";
    private readonly string noProjectsFoundMessage = "No projects found.";

    public ProjectRepository(IDocumentClient documentClient, IConfiguration configuration, IRepositoryBase<Project> repositoryBase)
    {
        this.repositoryBase = repositoryBase;
        this.configuration = configuration;
        this.documentClient = documentClient;

        databaseId = this.configuration["CosmosDbSettings:DatabaseName"]!;
        collectionId = "Projects";

        BuildCollection().Wait();
    }

    private async Task BuildCollection()
    {
        await documentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = databaseId });
        await documentClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(databaseId),
            new DocumentCollection { Id = collectionId });
    }

    public async Task<ResultData<PagedList<Project>>> GetProjects(ProjectParameters projectParameters)
    {
        var projects = documentClient.CreateDocumentQuery<Project>(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId)).AsQueryable();
    
        // Filtering results
        projects = FilterProjects(projects, projectParameters);

        // Sorting results
        if (!string.IsNullOrEmpty(projectParameters.Sort))
        {
            projects = repositoryBase.SortItems(projects, projectParameters);
        }

        if (projects is null)
        {
            return new NotFoundResultData<PagedList<Project>>(noProjectsFoundMessage);
        }

        // Paging results
        PagedList<Project> result = new(
               projects.ToList(),
               projects.Count(),
               projectParameters.PageNumber,
               projectParameters.PageSize);

        return new OkResultData<PagedList<Project>>(result);
    }

    public async Task<ResultData<Project>> GetProjectById(int projectId)
    {
        var project = documentClient.CreateDocumentQuery<Project>(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId), new FeedOptions { MaxItemCount = 1 }).Where(x => x.ProjectId == projectId).First();

        if (project is null)
        {
            return new NotFoundResultData<Project>(notFoundByIdMessage);
        }

        return new OkResultData<Project>(project);
    }

    public IQueryable<Project> FilterProjects(IQueryable<Project> projects, ProjectParameters projectParameters)
    {
        // Searching
        if (!string.IsNullOrEmpty(projectParameters.SearchByName))
        {
            projects = projects.Where(x => x.Name.Contains(projectParameters.SearchByName, StringComparison.CurrentCultureIgnoreCase));
        }

        if (projectParameters.SearchByStatus.HasValue)
        {
            projects = projects.Where(x => x.Status == projectParameters.SearchByStatus);
        }

        if (projectParameters.SearchByPriority.HasValue)
        {
            projects = projects.Where(x => x.Priority == projectParameters.SearchByPriority);
        }

        if (projectParameters.SearchByStartDate.HasValue)
        {
            projects = projects.Where(x => x.StartDate == DateOnly.FromDateTime((DateTime)projectParameters.SearchByStartDate));
        }

        if (projectParameters.SearchByCompletionDate.HasValue)
        {
            projects = projects.Where(x => x.CompletionDate == DateOnly.FromDateTime((DateTime)projectParameters.SearchByCompletionDate));
        }

        // Filtering
        if (projectParameters.MinStartDate.HasValue)
        {
            projects = projects.Where(x => x.StartDate >= DateOnly.FromDateTime((DateTime)projectParameters.MinStartDate));
        }

        if (projectParameters.MaxStartDate.HasValue)
        {
            projects = projects.Where(x => x.StartDate <= DateOnly.FromDateTime((DateTime)projectParameters.MaxStartDate));
        }

        if (projectParameters.MinCompletionDate.HasValue)
        {
            projects = projects.Where(x => x.CompletionDate >= DateOnly.FromDateTime((DateTime)projectParameters.MinCompletionDate));
        }

        if (projectParameters.MaxCompletionDate.HasValue)
        {
            projects = projects.Where(x => x.CompletionDate <= DateOnly.FromDateTime((DateTime)projectParameters.MaxCompletionDate));
        }

        return projects;
    }
}