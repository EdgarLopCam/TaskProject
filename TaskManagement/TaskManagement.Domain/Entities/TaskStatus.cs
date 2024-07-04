namespace TaskManagement.Domain.Entities
{
    public class TaskStatus
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public ICollection<Task> Tasks { get; set; }
    }
}