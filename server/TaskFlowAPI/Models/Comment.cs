using TaskFlowAPI.Interfaces;

namespace TaskFlowAPI.Models
{
    public class Comment : IAuditableEntity
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public string Content { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
        public Guid? UpdatedById { get; set; }

        // Navigation properties
        public TaskItem Task { get; set; }
        public User CreatedBy { get; set; }
        public User? UpdatedBy { get; set; }
    }
}
