﻿namespace TaskFlowAPI.DTOs
{
    public class TaskDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = null!;
        public Guid? AssignedToId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public Guid CreatedById { get; set; }
        public string CreatedByUsername { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string? UpdatedByUsername { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
    }
}
