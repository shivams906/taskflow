using TaskFlowAPI.Interfaces;
using TaskFlowAPI.Models.Enums;

namespace TaskFlowAPI.Models
{
    public class TaskItem : IAuditableEntity
    {
        public Guid Id { get; set; }

        public required string Title { get; set; }
        public string? Description { get; set; }

        public TaskItemStatus Status { get; set; } = TaskItemStatus.ToDo;

        public required Guid ProjectId { get; set; }
        public required Project Project { get; set; }

        public Guid? AssignedToId { get; set; }
        public User? AssignedTo { get; set; }

        public required Guid? CreatedById { get; set; }
        public required User? CreatedBy { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public Guid? UpdatedById { get; set; }
        public User? UpdatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<TaskTimeLog> TimeLogs { get; set; } = [];
        public ICollection<Comment> Comments { get; set; } = [];
    }
}
