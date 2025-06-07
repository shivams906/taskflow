namespace TaskFlowAPI.Models
{
    public class ChangeLog
    {
        public int Id { get; set; }
        public string EntityType { get; set; }
        public Guid EntityId { get; set; }
        public Guid? ChangedByUserId { get; set; }
        public User? ChangedByUser { get; set; }
        public string ChangeSummary { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
