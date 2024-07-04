namespace TaskManagement.Tests.Commands
{
    using Moq;
    using TaskManagement.Application.Commands;
    using TaskManagement.Application.Interfaces;
    using Task = System.Threading.Tasks.Task;

    public class CreateTaskCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ITaskFactory> _taskFactoryMock;
        private readonly CreateTaskCommandHandler _handler;

        public CreateTaskCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _taskFactoryMock = new Mock<ITaskFactory>();
            _handler = new CreateTaskCommandHandler(_unitOfWorkMock.Object, _taskFactoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_CreateTask_And_ReturnTaskId()
        {
            // Arrange
            var createTaskCommand = new CreateTaskCommand { Title = "Test Task", Description = "Test Description" };
            var task = new TaskManagement.Domain.Entities.Task { TaskId = 1, Title = "Test Task", Description = "Test Description" };

            _taskFactoryMock.Setup(f => f.CreateTaskAsync(createTaskCommand)).ReturnsAsync(task);
            _unitOfWorkMock.Setup(u => u.Tasks.AddAsync(task)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(createTaskCommand, CancellationToken.None);

            // Assert
            Assert.Equal(task.TaskId, result);
            _taskFactoryMock.Verify(f => f.CreateTaskAsync(createTaskCommand), Times.Once);
            _unitOfWorkMock.Verify(u => u.Tasks.AddAsync(task), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Call_UnitOfWork_And_TaskFactory()
        {
            // Arrange
            var createTaskCommand = new CreateTaskCommand { Title = "Test Task", Description = "Test Description" };
            var task = new TaskManagement.Domain.Entities.Task { TaskId = 1, Title = "Test Task", Description = "Test Description" };

            _taskFactoryMock.Setup(f => f.CreateTaskAsync(createTaskCommand)).ReturnsAsync(task);
            _unitOfWorkMock.Setup(u => u.Tasks.AddAsync(task)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(createTaskCommand, CancellationToken.None);

            // Assert
            _taskFactoryMock.Verify(f => f.CreateTaskAsync(createTaskCommand), Times.Once);
            _unitOfWorkMock.Verify(u => u.Tasks.AddAsync(task), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_TaskId()
        {
            // Arrange
            var createTaskCommand = new CreateTaskCommand { Title = "Test Task", Description = "Test Description" };
            var task = new TaskManagement.Domain.Entities.Task { TaskId = 1, Title = "Test Task", Description = "Test Description" };

            _taskFactoryMock.Setup(f => f.CreateTaskAsync(createTaskCommand)).ReturnsAsync(task);
            _unitOfWorkMock.Setup(u => u.Tasks.AddAsync(task)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(createTaskCommand, CancellationToken.None);

            // Assert
            Assert.Equal(task.TaskId, result);
        }
    }
}