namespace TaskManagement.Application.Commands
{
    using MediatR;
    using TaskManagement.Application.Interfaces;
    using TaskManagement.Domain.Exceptions;
    using Microsoft.EntityFrameworkCore;

    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTaskCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskId);
            if (task == null)
            {
                throw new NotFoundException(nameof(Task), request.TaskId);
            }

            if (!task.RowVersion.SequenceEqual(request.RowVersion))
            {
                throw new DbUpdateConcurrencyException("The task has been modified by another user.");
            }

            task.Title = request.Title;
            task.Description = request.Description;
            task.PriorityId = request.PriorityId;
            task.DueDate = request.DueDate;
            task.StatusId = request.StatusId;
            task.RowVersion = request.RowVersion;

            _unitOfWork.Tasks.UpdateAsync(task);

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DbUpdateConcurrencyException("Concurrency conflict detected. The task has been modified by another user.");
            }

            return Unit.Value;
        }
    }
}