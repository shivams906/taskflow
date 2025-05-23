namespace TaskFlowAPI.DTOs
{
    public record WorkspaceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string InviteCode { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
