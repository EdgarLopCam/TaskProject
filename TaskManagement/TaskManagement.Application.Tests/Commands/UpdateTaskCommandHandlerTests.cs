namespace TaskManagement.Tests.Commands
{
    using MediatR;
    using Moq;
    using TaskManagement.Application.Commands;
    using TaskManagement.Application.Interfaces;
    using TaskManagement.Domain.Exceptions;
    using Microsoft.EntityFrameworkCore;

    public class UpdateTaskCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpdateTaskCommandHandler _handler;

        public UpdateTaskCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new UpdateTaskCommandHandler(_unitOfWorkMock.Object);
        }

        [Theory]
        [MemberData(nameof(GetValidUpdateTaskCommands))]
        public async System.Threading.Tasks.Task Handle_Should_UpdateTask_When_TaskExists_And_RowVersionMatches(UpdateTaskCommand updateTaskCommand)
        {
            // Arrange
            var task = new TaskManagement.Domain.Entities.Task
            {
                TaskId = updateTaskCommand.TaskId,
                RowVersion = updateTaskCommand.RowVersion
            };

            _unitOfWorkMock.Setup(u => u.Tasks.GetByIdAsync(updateTaskCommand.TaskId)).ReturnsAsync(task);
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(updateTaskCommand, CancellationToken.None);

            // Assert
            Assert.Equal(Unit.Value, result);
            _unitOfWorkMock.Verify(u => u.Tasks.UpdateAsync(task), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Theory]
        [MemberData(nameof(GetNonExistentUpdateTaskCommands))]
        public async System.Threading.Tasks.Task Handle_Should_ThrowNotFoundException_When_TaskDoesNotExist(UpdateTaskCommand updateTaskCommand)
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Tasks.GetByIdAsync(updateTaskCommand.TaskId)).ReturnsAsync((TaskManagement.Domain.Entities.Task)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(updateTaskCommand, CancellationToken.None));
            _unitOfWorkMock.Verify(u => u.Tasks.UpdateAsync(It.IsAny<TaskManagement.Domain.Entities.Task>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Theory]
        [MemberData(nameof(GetMismatchedRowVersionUpdateTaskCommands))]
        public async System.Threading.Tasks.Task Handle_Should_ThrowDbUpdateConcurrencyException_When_RowVersionDoesNotMatch(UpdateTaskCommand updateTaskCommand)
        {
            // Arrange
            var task = new TaskManagement.Domain.Entities.Task
            {
                TaskId = updateTaskCommand.TaskId,
                RowVersion = new byte[] { 1, 2, 3, 4 }
            };

            _unitOfWorkMock.Setup(u => u.Tasks.GetByIdAsync(updateTaskCommand.TaskId)).ReturnsAsync(task);

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _handler.Handle(updateTaskCommand, CancellationToken.None));
            _unitOfWorkMock.Verify(u => u.Tasks.UpdateAsync(It.IsAny<TaskManagement.Domain.Entities.Task>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async System.Threading.Tasks.Task Handle_Should_ThrowDbUpdateConcurrencyException_When_ConcurrencyConflictOccursDuringUpdate()
        {
            // Arrange
            var updateTaskCommand = new UpdateTaskCommand
            {
                TaskId = 1,
                Title = "Test Task",
                Description = "Test Description",
                PriorityId = 1,
                DueDate = System.DateTime.UtcNow,
                StatusId = 1,
                RowVersion = new byte[] { 1, 2, 3, 4 }
            };

            var task = new TaskManagement.Domain.Entities.Task
            {
                TaskId = updateTaskCommand.TaskId,
                RowVersion = updateTaskCommand.RowVersion
            };

            _unitOfWorkMock.Setup(u => u.Tasks.GetByIdAsync(updateTaskCommand.TaskId)).ReturnsAsync(task);
            _unitOfWorkMock.Setup(u => u.CompleteAsync()).ThrowsAsync(new DbUpdateConcurrencyException());

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _handler.Handle(updateTaskCommand, CancellationToken.None));
            _unitOfWorkMock.Verify(u => u.Tasks.UpdateAsync(task), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        public static IEnumerable<object[]> GetValidUpdateTaskCommands()
        {
            yield return new object[]
            {
                new UpdateTaskCommand
                {
                    TaskId = 1,
                    Title = "Updated Task",
                    Description = "Updated Description",
                    PriorityId = 1,
                    DueDate = System.DateTime.UtcNow,
                    StatusId = 1,
                    RowVersion = new byte[] { 1, 2, 3, 4 }
                }
            };
        }

        public static IEnumerable<object[]> GetNonExistentUpdateTaskCommands()
        {
            yield return new object[]
            {
                new UpdateTaskCommand
                {
                    TaskId = 999, // Non-existent taskId
                    Title = "Non-existent Task",
                    Description = "Non-existent Description",
                    PriorityId = 1,
                    DueDate = System.DateTime.UtcNow,
                    StatusId = 1,
                    RowVersion = new byte[] { 1, 2, 3, 4 }
                }
            };
        }

        public static IEnumerable<object[]> GetMismatchedRowVersionUpdateTaskCommands()
        {
            yield return new object[]
            {
                new UpdateTaskCommand
                {
                    TaskId = 1,
                    Title = "Mismatched RowVersion Task",
                    Description = "Mismatched RowVersion Description",
                    PriorityId = 1,
                    DueDate = System.DateTime.UtcNow,
                    StatusId = 1,
                    RowVersion = new byte[] { 4, 3, 2, 1 }
                }
            };
        }
    }
}