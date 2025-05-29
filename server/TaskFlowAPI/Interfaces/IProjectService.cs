using TaskFlowAPI.DTOs;

namespace TaskFlowAPI.Interfaces;
public interface IProjectService
{
    Task<List<ProjectDto>> GetMyProjectsAsync(Guid userId);
    Task<bool> AddProjectUserAsync(Guid projectId, AddProjectUserDto dto, Guid currentUserId);
    Task<ProjectDto?> GetProjectByIdAsync(Guid projectId, Guid userId);
    Task<ProjectDto?> UpdateProjectAsync(Guid projectId, CreateProjectDto updated, Guid userId);
    Task<bool> DeleteProjectAsync(Guid projectId, Guid userId);
    Task<List<ProjectUserDto>> GetProjectUsersAsync(Guid projectId);
}
