using TaskFlowAPI.DTOs;

namespace TaskFlowAPI.Interfaces;
public interface IWorkspaceService
{
    Task<List<WorkspaceDto>> GetMyWorkspacesAsync(Guid userId);
    Task<WorkspaceDto?> GetWorkspaceByIdAsync(Guid workspaceId, Guid userId);
    Task<WorkspaceDto> UpdateWorkspaceAsync(Guid workspaceId, UpdateWorkspaceDto updateDto, Guid userId);
    Task<WorkspaceDto> CreateWorkspaceAsync(CreateWorkspaceDto newWorkspaceDto, Guid userId);
    Task DeleteWorkspaceAsync(Guid workspaceId, Guid userId);
    Task<WorkspaceDto> JoinWorkspaceAsync(string inviteCode, Guid userId);
    Task<List<ProjectDto>> GetProjectsForWorkspaceAsync(Guid workspaceId, Guid userId);
    Task<ProjectDto> CreateProjectAsync(Guid workspaceId, CreateProjectDto projectDto, Guid userId);
    Task<List<TaskDto>> GetMyTasksAsync(Guid workspaceId, Guid userId);
    Task<List<WorkspaceUserDto>> GetWorkspaceUsersAsync(Guid workspaceId);
}
