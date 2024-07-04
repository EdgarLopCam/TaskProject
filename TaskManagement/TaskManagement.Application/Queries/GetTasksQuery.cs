namespace TaskManagement.Application.Queries
{
    using MediatR;
    using TaskManagement.Application.Models;
    using Task = TaskManagement.Domain.Entities.Task;

    public class GetTasksQuery : IRequest<PagedResult<Task>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? Priority { get; set; }
        public int? Status { get; set; }
    }
}