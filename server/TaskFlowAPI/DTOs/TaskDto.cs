namespace TaskFlowAPI.DTOs
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
    }
}
