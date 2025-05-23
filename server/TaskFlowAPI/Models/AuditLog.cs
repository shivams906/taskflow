using TaskFlowAPI.Models.Enum;

namespace TaskFlowAPI.Models
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public required string TableName { get; set; }
        public string KeyValues { get; set; } = string.Empty; // e.g., "Id=123"
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? ChangedColumns { get; set; }
        public Guid? UserId { get; set; }
        public User? User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public AuditType AuditType { get; set; } = AuditType.None; // "Create", "Update", "Delete"
    }

}
