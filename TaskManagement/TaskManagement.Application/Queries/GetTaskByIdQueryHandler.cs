namespace TaskManagement.Application.Queries
{
    using MediatR;
    using TaskManagement.Application.Interfaces;
    using Task = TaskManagement.Domain.Entities.Task;

    public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, Task>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTaskByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Task> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Tasks.GetByIdAsync(request.Id);
        }
    }
}
