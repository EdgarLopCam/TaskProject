namespace TaskManagement.Application.Factories
{
    using TaskManagement.Application.Commands;
    using TaskManagement.Application.Interfaces;
    using Task = TaskManagement.Domain.Entities.Task;

    public class TaskFactory : ITaskFactory
    {
        public async Task<Task> CreateTaskAsync(CreateTaskCommand command)
        {
            var task = new Task
            {
                Title = command.Title,
                Description = command.Description,
                PriorityId = command.PriorityId,
                DueDate = command.DueDate,
                StatusId = command.StatusId
            };
            return await System.Threading.Tasks.Task.FromResult(task);
        }
    }
}
