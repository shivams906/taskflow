namespace TaskFlowAPI.DTOs
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public Guid WorkspaceId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public Guid CreatedById { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }

    public class AdminDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = null!;
    }
}
