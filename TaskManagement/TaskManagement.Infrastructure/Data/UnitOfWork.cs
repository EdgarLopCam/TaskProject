namespace TaskManagement.Infrastructure.Data
{
    using System.Threading.Tasks;
    using TaskManagement.Application.Interfaces;
    using TaskManagement.Domain.Entities;
    using Task = TaskManagement.Domain.Entities.Task;
    using TaskStatus = TaskManagement.Domain.Entities.TaskStatus;

    public class UnitOfWork : IUnitOfWork
    {
        private readonly TaskDbContext _context;
        private Repository<Task> _tasks;
        private Repository<TaskPriority> _taskPriorities;
        private Repository<TaskStatus> _taskStatuses;

        public UnitOfWork(TaskDbContext context)
        {
            _context = context;
        }

        public IRepository<Task> Tasks => _tasks ??= new Repository<Task>(_context);

        public IRepository<TaskPriority> TaskPriorities => _taskPriorities ??= new Repository<TaskPriority>(_context);

        public IRepository<TaskStatus> TaskStatuses => _taskStatuses ??= new Repository<TaskStatus>(_context);

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}