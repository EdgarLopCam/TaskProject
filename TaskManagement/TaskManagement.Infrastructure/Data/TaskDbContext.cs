namespace TaskManagement.Infrastructure.Data
{
    using Microsoft.EntityFrameworkCore;
    using TaskManagement.Domain.Entities;
    using Task = TaskManagement.Domain.Entities.Task;
    using TaskStatus = TaskManagement.Domain.Entities.TaskStatus;

    public class TaskDbContext : DbContext
    {
        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options) { }

        public DbSet<Task> Tasks { get; set; }
        public DbSet<TaskPriority> TaskPriorities { get; set; }
        public DbSet<TaskStatus> TaskStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de Task
            modelBuilder.Entity<Task>(entity =>
            {
                entity.HasKey(e => e.TaskId);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Description).HasMaxLength(2000);
                entity.Property(e => e.RowVersion).IsRowVersion();
                entity.HasOne(e => e.Priority)
                      .WithMany(p => p.Tasks)
                      .HasForeignKey(e => e.PriorityId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Status)
                      .WithMany(s => s.Tasks)
                      .HasForeignKey(e => e.StatusId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.DueDate);
                entity.HasIndex(e => e.PriorityId);
                entity.HasIndex(e => e.StatusId);
                entity.HasIndex(e => new { e.Title, e.PriorityId }).IsUnique();
            });

            // Configuración de TaskPriority
            modelBuilder.Entity<TaskPriority>(entity =>
            {
                entity.HasKey(e => e.PriorityId);
                entity.Property(e => e.PriorityName).IsRequired().HasMaxLength(50);
            });

            // Configuración de TaskStatus
            modelBuilder.Entity<TaskStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId);
                entity.Property(e => e.StatusName).IsRequired().HasMaxLength(50);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}