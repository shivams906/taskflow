namespace TaskFlowAPI.DTOs
{
    public record WorkspaceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string InviteCode { get; set; }
        public Guid CreatedById { get; set; }
        public string CreatedByUsername { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public Guid? UpdatedById { get; set; }
        public string? UpdatedByUsername { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
    }
}
