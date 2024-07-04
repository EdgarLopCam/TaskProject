namespace TaskManagement.Application.Commands
{
    using MediatR;
    using TaskManagement.Application.Interfaces;
    using TaskManagement.Domain.Exceptions;
    using Microsoft.EntityFrameworkCore;

    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTaskCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskId);
            if (task == null)
            {
                throw new NotFoundException(nameof(Task), request.TaskId);
            }

            // Check the RowVersion for concurrency
            if (!task.RowVersion.SequenceEqual(request.RowVersion))
            {
                throw new DbUpdateConcurrencyException("The task has been modified by another user.");
            }

            _unitOfWork.Tasks.DeleteAsync(task);

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
