namespace TaskManagement.Application.Commands
{
    using MediatR;
    using TaskManagement.Application.Interfaces;

    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITaskFactory _taskFactory;

        public CreateTaskCommandHandler(IUnitOfWork unitOfWork, ITaskFactory taskFactory)
        {
            _unitOfWork = unitOfWork;
            _taskFactory = taskFactory;
        }

        public async Task<int> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskFactory.CreateTaskAsync(request);
            await _unitOfWork.Tasks.AddAsync(task);
            await _unitOfWork.CompleteAsync();

            return task.TaskId;
        }
    }
}