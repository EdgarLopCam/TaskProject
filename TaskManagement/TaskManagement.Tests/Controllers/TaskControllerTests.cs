namespace TaskManagement.Tests.Controllers
{
    using Moq;
    using TaskManagement.API.Controllers;
    using TaskManagement.Application.Commands;
    using TaskManagement.Application.Queries;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using TaskManagement.Application.Models;

    public class TaskControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly TaskController _controller;

        public TaskControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new TaskController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetTaskById_ReturnsOkResult_WhenTaskExists()
        {
            // Arrange
            var taskId = 1;
            var task = new TaskManagement.Domain.Entities.Task { TaskId = taskId, Title = "Test Task" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetTaskByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(task);

            // Act
            var result = await _controller.GetTaskById(taskId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<TaskManagement.Domain.Entities.Task>(okResult.Value);
            Assert.Equal(taskId, returnValue.TaskId);
        }

        [Fact]
        public async Task GetTaskById_ReturnsNotFound_WhenTaskDoesNotExist()
        {
            // Arrange
            var taskId = 1;
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetTaskByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((TaskManagement.Domain.Entities.Task)null);

            // Act
            var result = await _controller.GetTaskById(taskId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetTasks_ReturnsOkResult_WithPagedResult()
        {
            // Arrange
            var query = new GetTasksQuery { PageNumber = 1, PageSize = 10 };
            var pagedResult = new PagedResult<TaskManagement.Domain.Entities.Task>
            {
                Items = new List<TaskManagement.Domain.Entities.Task> { new TaskManagement.Domain.Entities.Task { TaskId = 1, Title = "Test Task" } },
                PageNumber = 1,
                PageSize = 10,
                TotalRecords = 1
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetTasksQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetTasks(query.PageNumber, query.PageSize, query.Priority, query.Status);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PagedResult<TaskManagement.Domain.Entities.Task>>(okResult.Value);
            Assert.Equal(query.PageNumber, returnValue.PageNumber);
        }

        [Fact]
        public async Task CreateTask_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var command = new CreateTaskCommand { Title = "New Task" };
            var newTaskId = 1;
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateTaskCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(newTaskId);

            // Act
            var result = await _controller.CreateTask(command);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(newTaskId, createdAtActionResult.RouteValues["id"]);
        }

        [Fact]
        public async Task UpdateTask_ReturnsNoContentResult_WhenTaskIsUpdated()
        {
            // Arrange
            var taskId = 1;
            var command = new UpdateTaskCommand { TaskId = taskId, Title = "Updated Task" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateTaskCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.UpdateTask(taskId, command);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateTask_ReturnsBadRequest_WhenTaskIdDoesNotMatch()
        {
            // Arrange
            var taskId = 1;
            var command = new UpdateTaskCommand { TaskId = 2, Title = "Updated Task" };

            // Act
            var result = await _controller.UpdateTask(taskId, command);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteTask_ReturnsNoContentResult_WhenTaskIsDeleted()
        {
            // Arrange
            var taskId = 1;
            var command = new DeleteTaskCommand { TaskId = taskId };
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteTaskCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.DeleteTask(taskId, command);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteTask_ReturnsBadRequest_WhenTaskIdDoesNotMatch()
        {
            // Arrange
            var taskId = 1;
            var command = new DeleteTaskCommand { TaskId = 2 };

            // Act
            var result = await _controller.DeleteTask(taskId, command);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
    }
}