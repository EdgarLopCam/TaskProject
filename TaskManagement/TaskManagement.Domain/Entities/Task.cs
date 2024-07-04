namespace TaskManagement.Domain.Entities
{
    public class Task
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int PriorityId { get; set; }
        public DateTime? DueDate { get; set; }
        public int StatusId { get; set; }
        public byte[] RowVersion { get; set; }
        public TaskPriority Priority { get; set; }
        public TaskStatus Status { get; set; }
    }
}