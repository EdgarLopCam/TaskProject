namespace TaskManagement.Application.Commands
{
    using MediatR;

    public class DeleteTaskCommand : IRequest
    {
        public int TaskId { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
