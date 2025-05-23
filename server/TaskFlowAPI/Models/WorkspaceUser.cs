using TaskFlowAPI.Interfaces;
using TaskFlowAPI.Models.Enum;

namespace TaskFlowAPI.Models
{
    public class WorkspaceUser : IAuditableEntity
    {
        public Guid Id { get; set; }
        public Guid WorkspaceId { get; set; }
        public Workspace Workspace { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid? CreatedById { get; set; }
        public User? CreatedBy { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public Guid? UpdatedById { get; set; }
        public User? UpdatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }

        public WorkspaceRole Role { get; set; } = WorkspaceRole.Admin;
    }
}
