namespace TaskManagement.Application.Interfaces
{
    using TaskManagement.Application.Commands;
    using Task = TaskManagement.Domain.Entities.Task;

    public interface ITaskFactory
    {
        System.Threading.Tasks.Task<Task> CreateTaskAsync(CreateTaskCommand command);
    }
}
