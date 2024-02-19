using CommonUtils.ResultDataResponse;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using TaskTracker_DAL.Interfaces;
using TaskTracker_DAL.Models;

namespace TaskTracker_DAL.Repositories;

public class TaskRepository: ITaskRepository
{
    private readonly IRepositoryBase<TaskUnit> repositoryBase;
    private readonly IConfiguration configuration;
    private readonly IDocumentClient documentClient;
    readonly string databaseId;
    readonly string collectionId;

    private readonly string notFoundByIdMessage = "Task with given id not found.";
    private readonly string noTasksFoundMessage = "No tasks found.";

    public TaskRepository(IDocumentClient documentClient, IConfiguration configuration, IRepositoryBase<TaskUnit> repositoryBase)
    {
        this.repositoryBase = repositoryBase;
        this.configuration = configuration;
        this.documentClient = documentClient;

        databaseId = this.configuration["CosmosDbSettings:DatabaseName"]!;
        collectionId = "Tasks";

        BuildCollection().Wait();
    }

    private async Task BuildCollection()
    {
        await documentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = databaseId });
        await documentClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(databaseId),
            new DocumentCollection { Id = collectionId });
    }

    public async Task<ResultData<PagedList<TaskUnit>>> GetTasks(TaskParameters taskParameters)
    {
        var tasks = documentClient.CreateDocumentQuery<TaskUnit>(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId)).AsQueryable();

        // Filtering results
        tasks = FilterTasks(tasks, taskParameters);

        // Sorting results
        if (!string.IsNullOrEmpty(taskParameters.Sort))
        {
            tasks = repositoryBase.SortItems(tasks, taskParameters);
        }

        if (!tasks.Any())
        {
            return new NotFoundResultData<PagedList<TaskUnit>>(noTasksFoundMessage);
        }

        // Paging results
        PagedList<TaskUnit> result = new(
            tasks.ToList(),
            tasks.Count(),
            taskParameters.PageNumber,
            taskParameters.PageSize
            );

        return new OkResultData<PagedList<TaskUnit>>(result);
    }

    public async Task<ResultData<TaskUnit>> GetTaskById(int taskId)
    {
        var task = documentClient.CreateDocumentQuery<TaskUnit>(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId), new FeedOptions { MaxItemCount = 1 }).Where(x => x.TaskId == taskId).First();

        if (task is null)
        {
            return new NotFoundResultData<TaskUnit>(notFoundByIdMessage);
        }

        return new OkResultData<TaskUnit>(task);
    }

    public IQueryable<TaskUnit> FilterTasks(IQueryable<TaskUnit> tasks, TaskParameters taskParameters)
    {
        // Searching
        if (taskParameters.ProjectId is not null)
        {
            tasks = tasks.Where(x => x.ProjectId == taskParameters.ProjectId);
        }

        return tasks;
    }
}