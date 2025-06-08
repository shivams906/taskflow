using TaskFlowAPI.DTOs;
using TaskFlowAPI.Models;

namespace TaskFlowAPI.Interfaces
{
    public interface ITaskService
    {
        Task<PagedResult<TaskDto>> GetTasksForProjectAsync(Guid projectId, Guid userId, QueryParams queryParams);
        Task<TaskDto> GetTaskByIdAsync(Guid taskId, Guid userId);
        Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, Guid userId);
        Task<string> UpdateTaskStatusAsync(Guid taskId, UpdateTaskStatusDto dto, Guid userId);
        Task<string> LogTimeAsync(Guid taskId, CreateTimeLogDto dto, Guid userId);
        Task<List<TimeLogDto>> GetTimeLogsAsync(Guid taskId, Guid userId, bool onlyMine);
        Task<TaskDto> UpdateTaskAsync(Guid taskId, CreateTaskDto updated, Guid userId);
        Task<string> DeleteTaskAsync(Guid taskId, Guid userId);
        Task<string> AssignTaskAsync(Guid taskId, AssignUserToTaskDto dto, Guid userId);
        Task<string> UnassignTaskAsync(Guid taskId, Guid userId);
    }
}
