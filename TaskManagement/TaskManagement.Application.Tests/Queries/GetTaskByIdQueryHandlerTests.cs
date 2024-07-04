namespace TaskManagement.Tests.Queries
{
    using Moq;
    using TaskManagement.Application.Queries;
    using TaskManagement.Application.Interfaces;

    public class GetTaskByIdQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly GetTaskByIdQueryHandler _handler;

        public GetTaskByIdQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new GetTaskByIdQueryHandler(_unitOfWorkMock.Object);
        }

        [Fact]
        public async System.Threading.Tasks.Task Handle_Should_ReturnTask_When_TaskExists()
        {
            // Arrange
            var taskId = 1;
            var task = new TaskManagement.Domain.Entities.Task { TaskId = taskId };
            _unitOfWorkMock.Setup(u => u.Tasks.GetByIdAsync(taskId)).ReturnsAsync(task);

            var query = new GetTaskByIdQuery { Id = taskId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taskId, result.TaskId);
        }

        [Fact]
        public async System.Threading.Tasks.Task Handle_Should_ReturnNull_When_TaskDoesNotExist()
        {
            // Arrange
            var taskId = 1;
            _unitOfWorkMock.Setup(u => u.Tasks.GetByIdAsync(taskId)).ReturnsAsync((TaskManagement.Domain.Entities.Task)null);

            var query = new GetTaskByIdQuery { Id = taskId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async System.Threading.Tasks.Task Handle_Should_ReturnCorrectTask_ForDifferentTaskIds(int taskId)
        {
            // Arrange
            var task = new TaskManagement.Domain.Entities.Task { TaskId = taskId };
            _unitOfWorkMock.Setup(u => u.Tasks.GetByIdAsync(taskId)).ReturnsAsync(task);

            var query = new GetTaskByIdQuery { Id = taskId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taskId, result.TaskId);
        }
    }
}