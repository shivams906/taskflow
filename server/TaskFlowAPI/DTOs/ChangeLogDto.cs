namespace TaskFlowAPI.DTOs
{
    public record ChangeLogDto
    {
        public string EntityType { get; set; }
        public Guid EntityId { get; set; }
        public Guid ChangedByUserId { get; set; }
        public string ChangedByUserName { get; set; }
        public string ChangeSummary { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
