namespace TaskManagement.Tests.Commands
{
    using MediatR;
    using Moq;
    using TaskManagement.Application.Commands;
    using TaskManagement.Application.Interfaces;
    using TaskManagement.Domain.Exceptions;
    using Microsoft.EntityFrameworkCore;
    using Task = System.Threading.Tasks.Task;

    public class DeleteTaskCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly DeleteTaskCommandHandler _handler;

        public DeleteTaskCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new DeleteTaskCommandHandler(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_Should_DeleteTask_When_TaskExists_And_RowVersionMatches()
        {
            // Arrange
            var taskId = 1;
            var rowVersion = new byte[] { 1, 2, 3, 4 };
            var task = new TaskManagement.Domain.Entities.Task { TaskId = taskId, RowVersion = rowVersion };
            var deleteTaskCommand = new DeleteTaskCommand { TaskId = taskId, RowVersion = rowVersion };

            _unitOfWorkMock.Setup(u => u.Tasks.GetByIdAsync(taskId)).ReturnsAsync(task);
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(deleteTaskCommand, CancellationToken.None);

            // Assert
            Assert.Equal(Unit.Value, result);
            _unitOfWorkMock.Verify(u => u.Tasks.DeleteAsync(task), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ThrowNotFoundException_When_TaskDoesNotExist()
        {
            // Arrange
            var taskId = 1;
            var rowVersion = new byte[] { 1, 2, 3, 4 };
            var deleteTaskCommand = new DeleteTaskCommand { TaskId = taskId, RowVersion = rowVersion };

            _unitOfWorkMock.Setup(u => u.Tasks.GetByIdAsync(taskId)).ReturnsAsync((TaskManagement.Domain.Entities.Task)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(deleteTaskCommand, CancellationToken.None));
            _unitOfWorkMock.Verify(u => u.Tasks.DeleteAsync(It.IsAny<TaskManagement.Domain.Entities.Task>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_ThrowDbUpdateConcurrencyException_When_RowVersionDoesNotMatch()
        {
            // Arrange
            var taskId = 1;
            var rowVersion = new byte[] { 1, 2, 3, 4 };
            var task = new TaskManagement.Domain.Entities.Task { TaskId = taskId, RowVersion = new byte[] { 4, 3, 2, 1 } };
            var deleteTaskCommand = new DeleteTaskCommand { TaskId = taskId, RowVersion = rowVersion };

            _unitOfWorkMock.Setup(u => u.Tasks.GetByIdAsync(taskId)).ReturnsAsync(task);

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _handler.Handle(deleteTaskCommand, CancellationToken.None));
            _unitOfWorkMock.Verify(u => u.Tasks.DeleteAsync(It.IsAny<TaskManagement.Domain.Entities.Task>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_ThrowDbUpdateConcurrencyException_When_ConcurrencyConflictOccursDuringDeletion()
        {
            // Arrange
            var taskId = 1;
            var rowVersion = new byte[] { 1, 2, 3, 4 };
            var task = new TaskManagement.Domain.Entities.Task { TaskId = taskId, RowVersion = rowVersion };
            var deleteTaskCommand = new DeleteTaskCommand { TaskId = taskId, RowVersion = rowVersion };

            _unitOfWorkMock.Setup(u => u.Tasks.GetByIdAsync(taskId)).ReturnsAsync(task);
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ThrowsAsync(new DbUpdateConcurrencyException());

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _handler.Handle(deleteTaskCommand, CancellationToken.None));
            _unitOfWorkMock.Verify(u => u.Tasks.DeleteAsync(task), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }
    }
}