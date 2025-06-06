﻿using TaskFlowAPI.Interfaces;

namespace TaskFlowAPI.Models
{
    public class User : IAuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = "User";
        public Guid? CreatedById { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<WorkspaceUser> WorkspaceUsers { get; set; } = [];
        public ICollection<ProjectUser> ProjectUsers { get; set; } = [];
        public ICollection<TaskItem> AssignedTasks { get; set; } = [];
    }

}
