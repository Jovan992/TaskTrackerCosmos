﻿using CommonUtils.ResultDataResponse;
using NSubstitute;
using TaskTracker_BL.DTOs;
using TaskTracker_BL.Interfaces;
using TaskTracker_BL.Services;
using TaskTracker_DAL.Interfaces;
using TaskTracker_DAL.Models;

namespace TaskTracker_BL_Tests.Services;

public class TaskServiceTests
{
    private readonly ITaskRepository taskRepository;
    private readonly ITaskService taskService;
    private readonly string noTasksFoundMessage = "No tasks found.";
    private readonly string notFoundTaskByIdMessage = "Task with given id not found.";
    private readonly int taskId = (new Random()).Next();

    public TaskServiceTests()
    {
        taskRepository = Substitute.For<ITaskRepository>();
        taskService = new TaskService(taskRepository);
    }

    [Fact]
    public async Task GetTaskById_ShouldReturnOkResultData_WhenTaskFound()
    {
        // Arrange
        var task = new TaskUnit
        {
            ProjectId = 1,
            Description = "test1",
            Name = "Test1",
            TaskId = taskId
        };

        var expectedResultData = new OkResultData<TaskUnit>(task);

        taskRepository.GetTaskById(taskId).Returns(expectedResultData);

        // Act
        var result = await taskService.GetTaskById(taskId);

        // Assert
        Assert.IsType<OkResultData<TaskDto>>(result);
        Assert.Equal(taskId, result.Data!.TaskId);
    }

    [Fact]
    public async Task GetTaskById_ShouldReturnNotFoundResultData_WhenTaskIsNotFound()
    {
        // Arrange
        var expectedResultData = new NotFoundResultData<TaskUnit>(notFoundTaskByIdMessage);

        taskRepository.GetTaskById(Arg.Any<int>()).Returns(expectedResultData);

        // Act
        var result = await taskService.GetTaskById(taskId);

        // Assert
        Assert.IsType<NotFoundResultData<TaskDto>>(result);
        Assert.Equal(notFoundTaskByIdMessage, result.Message);
    }

    [Fact]
    public async Task GetTasks_ShouldReturnOkResultData_WhenTaskExists()
    {
        // Arrange
        var parameters = new TaskParameters();
        var tasks = new List<TaskUnit>()
        {
            new ()
            {
                ProjectId = 1,
                Description = "test1",
                Name = "Test1",
                TaskId = taskId
            },
            new ()
            {
                ProjectId = 1,
                Description = "test1",
                Name = "Test1",
                TaskId = taskId+1
            },
            new ()
            {
                ProjectId = 1,
                Description = "test1",
                Name = "Test1",
                TaskId = taskId+2
            },
        };
        var pagedListOfTasks = new PagedList<TaskUnit>(tasks, tasks.Count, parameters.PageNumber, parameters.PageSize);

        var expectedResultData = new OkResultData<PagedList<TaskUnit>>(pagedListOfTasks);

        taskRepository.GetTasks(Arg.Any<TaskParameters>()).Returns(expectedResultData);

        // Act
        var result = await taskService.GetTasks(parameters);

        // Assert
        Assert.IsType<OkResultData<PagedList<TaskDto>>>(result);
        Assert.NotNull(expectedResultData.Data);
    }

    [Fact]
    public async Task GetTasks_ShouldReturnNotFoundResultData_WhenTaskDoesntExist()
    {
        // Arrange
        var parameters = new TaskParameters();
        var expectedResultData = new NotFoundResultData<PagedList<TaskUnit>>(noTasksFoundMessage);

        taskRepository.GetTasks(Arg.Any<TaskParameters>()).Returns(expectedResultData);

        // Act
        var result = await taskService.GetTasks(parameters);

        // Assert
        Assert.IsType<NotFoundResultData<PagedList<TaskDto>>>(result);
        Assert.Equal(noTasksFoundMessage, result.Message);
    }
    }
