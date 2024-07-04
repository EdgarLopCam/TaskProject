namespace TaskManagement.Application.Interfaces
{
    using TaskManagement.Domain.Entities;
    using Task = TaskManagement.Domain.Entities.Task;
    using TaskStatus = TaskManagement.Domain.Entities.TaskStatus;

    public interface IUnitOfWork : IDisposable
    {
        IRepository<Task> Tasks { get; }
        IRepository<TaskPriority> TaskPriorities { get; }
        IRepository<TaskStatus> TaskStatuses { get; }
        Task<int> CompleteAsync();
    }
}