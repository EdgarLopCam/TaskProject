namespace TaskManagement.Application.Commands
{
    using MediatR;

    public class CreateTaskCommand : IRequest<int>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int PriorityId { get; set; }
        public DateTime? DueDate { get; set; }
        public int StatusId { get; set; }
    }
}
