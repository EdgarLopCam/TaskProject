namespace TaskManagement.Domain.Entities
{
    public class TaskPriority
    {
        public int PriorityId { get; set; }
        public string PriorityName { get; set; }
        public ICollection<Task> Tasks { get; set; }
    }
}