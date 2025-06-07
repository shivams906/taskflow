namespace TaskFlowAPI.Interfaces
{
    public interface IRoleAccessService
    {
        Task<bool> HasPermissionAsync<TPermission>(Guid userId, TPermission permission, Guid resourceId) where TPermission : Enum;
        Task<List<string>> GetPermissionsForWorkspaceAsync(Guid userId, Guid workspaceId);
        Task<List<string>> GetPermissionsForProjectAsync(Guid userId, Guid projectId);
        Task<List<string>> GetPermissionsForTaskAsync(Guid userId, Guid taskId);
    }

}
