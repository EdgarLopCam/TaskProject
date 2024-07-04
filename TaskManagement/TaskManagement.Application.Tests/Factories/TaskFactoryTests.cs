namespace TaskManagement.Tests.Factories
{
    using TaskManagement.Application.Commands;
    using TaskFactory = TaskManagement.Application.Factories.TaskFactory;

    public class TaskFactoryTests
    {
        private readonly TaskFactory _taskFactory;

        public TaskFactoryTests()
        {
            _taskFactory = new TaskFactory();
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateTaskAsync_Should_CreateTask_When_ValidCommand()
        {
            // Arrange
            var command = new CreateTaskCommand
            {
                Title = "Test Task",
                Description = "Test Description",
                PriorityId = 1,
                DueDate = System.DateTime.UtcNow,
                StatusId = 1
            };

            // Act
            var task = await _taskFactory.CreateTaskAsync(command);

            // Assert
            Assert.NotNull(task);
            Assert.Equal(command.Title, task.Title);
            Assert.Equal(command.Description, task.Description);
            Assert.Equal(command.PriorityId, task.PriorityId);
            Assert.Equal(command.DueDate, task.DueDate);
            Assert.Equal(command.StatusId, task.StatusId);
        }

        [Theory]
        [InlineData("Title1", "Description1", 1, "2023-12-01", 1)]
        [InlineData("Title2", "Description2", 2, "2023-12-02", 2)]
        [InlineData("Title3", "Description3", 3, "2023-12-03", 3)]
        public async System.Threading.Tasks.Task CreateTaskAsync_Should_CreateTask_With_InlineData(string title, string description, int priorityId, string dueDate, int statusId)
        {
            // Arrange
            var command = new CreateTaskCommand
            {
                Title = title,
                Description = description,
                PriorityId = priorityId,
                DueDate = System.DateTime.Parse(dueDate),
                StatusId = statusId
            };

            // Act
            var task = await _taskFactory.CreateTaskAsync(command);

            // Assert
            Assert.NotNull(task);
            Assert.Equal(command.Title, task.Title);
            Assert.Equal(command.Description, task.Description);
            Assert.Equal(command.PriorityId, task.PriorityId);
            Assert.Equal(command.DueDate, task.DueDate);
            Assert.Equal(command.StatusId, task.StatusId);
        }
    }
}