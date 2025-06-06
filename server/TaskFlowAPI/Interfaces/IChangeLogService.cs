using TaskFlowAPI.DTOs;

namespace TaskFlowAPI.Interfaces
{
    public interface IChangeLogService
    {
        public Task LogChange(string entityType, Guid entityId, Guid userId, string summary);
        public Task<List<ChangeLogDto>> GetChangeLogs(string entityType, Guid entityId);
    }
}
