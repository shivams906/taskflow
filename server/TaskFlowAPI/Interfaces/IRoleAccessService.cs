namespace TaskFlowAPI.Interfaces
{
    public interface IRoleAccessService
    {
        Task<bool> CanEditWorkspaceAsync(Guid userId, Guid workspaceId);
        Task<bool> CanViewWorkspaceAsync(Guid userId, Guid workspaceId);
        Task<bool> CanDeleteWorkspaceAsync(Guid userId, Guid workspaceId);

        Task<bool> CanCreateProjectAsync(Guid userId, Guid workspaceId);
        Task<bool> CanEditProjectAsync(Guid userId, Guid projectId);
        Task<bool> CanViewProjectAsync(Guid userId, Guid projectId);
        Task<bool> CanDeleteProjectAsync(Guid userId, Guid projectId);

        Task<bool> CanCreateTaskAsync(Guid userId, Guid projectId);
        Task<bool> CanEditTaskAsync(Guid userId, Guid taskId);
        Task<bool> CanUpdateTaskStatusAsync(Guid userId, Guid taskId);
        Task<bool> CanViewTaskAsync(Guid userId, Guid taskId);
        Task<bool> CanDeleteTaskAsync(Guid userId, Guid taskId);
        Task<bool> CanLogTimeAsync(Guid userId, Guid taskId);
    }

}
