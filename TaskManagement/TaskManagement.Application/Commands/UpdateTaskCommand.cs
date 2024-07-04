namespace TaskManagement.Application.Commands
{
    using MediatR;

    public class UpdateTaskCommand : IRequest
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int PriorityId { get; set; }
        public DateTime? DueDate { get; set; }
        public int StatusId { get; set; }
        public byte[] RowVersion { get; set; }
    }
}