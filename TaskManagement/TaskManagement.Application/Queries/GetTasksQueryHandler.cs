namespace TaskManagement.Application.Queries
{
    using MediatR;
    using TaskManagement.Application.Interfaces;
    using TaskManagement.Application.Models;
    using Microsoft.EntityFrameworkCore;
    using Task = TaskManagement.Domain.Entities.Task;

    public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, PagedResult<Task>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTasksQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<Task>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Tasks.GetAll();

            if (request.Priority.HasValue)
            {
                query = query.Where(t => t.PriorityId == request.Priority.Value);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(t => t.StatusId == request.Status.Value);
            }

            var totalRecords = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Task>
            {
                Items = items,
                TotalRecords = totalRecords,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
