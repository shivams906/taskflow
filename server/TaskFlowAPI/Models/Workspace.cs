using System.ComponentModel.DataAnnotations;
using TaskFlowAPI.Interfaces;

namespace TaskFlowAPI.Models
{
    public class Workspace : IAuditableEntity
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;
        public string InviteCode { get; set; }
        public Guid? CreatedById { get; set; }
        public User? CreatedBy { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public Guid? UpdatedById { get; set; }
        public User? UpdatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public ICollection<WorkspaceUser> WorkspaceUsers { get; set; } = [];
        public ICollection<Project> Projects { get; set; } = [];
    }

}