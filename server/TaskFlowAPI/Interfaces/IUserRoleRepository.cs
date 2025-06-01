using TaskFlowAPI.Models.Enum;
using TaskFlowAPI.Models.Enums;

namespace TaskFlowAPI.Interfaces
{
    public interface IUserRoleRepository
    {
        Task<WorkspaceRole?> GetWorkspaceRoleAsync(Guid userId, Guid workspaceId);
        Task<ProjectRole?> GetProjectRoleAsync(Guid userId, Guid projectId);
        Task<bool> IsAssigneeAsync(Guid userId, Guid taskId);
        Task<bool> IsTaskCreatorAsync(Guid userId, Guid taskId);
        Task<Guid> GetProjectIdFromTaskAsync(Guid taskId);
        Task<Guid> GetWorkspaceIdFromProjectAsync(Guid projectId);
    }
}
