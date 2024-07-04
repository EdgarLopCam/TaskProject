namespace TaskManagement.Application.Queries
{
    using MediatR;
    using Task = TaskManagement.Domain.Entities.Task;

    public class GetTaskByIdQuery : IRequest<Task>
    {
        public int Id { get; set; }
    }
}
