using TaskFlowAPI.Interfaces;
using TaskFlowAPI.Models.Enum;

namespace TaskFlowAPI.Services
{
    public class RoleAccessService : IRoleAccessService
    {
        private readonly IUserRoleRepository _roleRepo;

        public RoleAccessService(IUserRoleRepository roleRepo)
        {
            _roleRepo = roleRepo;
        }

        public async Task<bool> HasPermissionAsync<TPermission>(Guid userId, TPermission permission, Guid resourceId)
            where TPermission : Enum
        {
            return permission switch
            {
                WorkspacePermission.ManageWorkspace => await CanEditWorkspaceAsync(userId, resourceId),
                WorkspacePermission.ViewWorkspace => await CanViewWorkspaceAsync(userId, resourceId),
                WorkspacePermission.DeleteWorkspace => await CanDeleteWorkspaceAsync(userId, resourceId),

                ProjectPermission.ManageProject => await CanEditProjectAsync(userId, resourceId),
                ProjectPermission.ViewProject => await CanViewProjectAsync(userId, resourceId),
                ProjectPermission.DeleteProject => await CanDeleteProjectAsync(userId, resourceId),

                TaskPermission.ManageTask => await CanEditTaskAsync(userId, resourceId),
                TaskPermission.ViewTask => await CanViewTaskAsync(userId, resourceId),
                TaskPermission.DeleteTask => await CanDeleteTaskAsync(userId, resourceId),
                TaskPermission.UpdateTaskStatus => await CanUpdateTaskStatusAsync(userId, resourceId),
                TaskPermission.LogTime => await CanLogTimeAsync(userId, resourceId),

                _ => false
            };
        }

        // Workspace Logic
        private async Task<bool> CanEditWorkspaceAsync(Guid userId, Guid workspaceId)
        {
            var role = await _roleRepo.GetWorkspaceRoleAsync(userId, workspaceId);
            return role == WorkspaceRole.Owner || role == WorkspaceRole.Admin;
        }

        private async Task<bool> CanViewWorkspaceAsync(Guid userId, Guid workspaceId)
        {
            var role = await _roleRepo.GetWorkspaceRoleAsync(userId, workspaceId);
            return role != null;
        }

        private async Task<bool> CanDeleteWorkspaceAsync(Guid userId, Guid workspaceId)
        {
            var role = await _roleRepo.GetWorkspaceRoleAsync(userId, workspaceId);
            return role == WorkspaceRole.Owner;
        }

        // Project Logic
        private async Task<bool> CanEditProjectAsync(Guid userId, Guid projectId)
        {
            var projRole = await _roleRepo.GetProjectRoleAsync(userId, projectId);
            var workspaceId = await _roleRepo.GetWorkspaceIdFromProjectAsync(projectId);
            var wsRole = await _roleRepo.GetWorkspaceRoleAsync(userId, workspaceId);

            return projRole == ProjectRole.Owner || projRole == ProjectRole.Admin ||
                   wsRole == WorkspaceRole.Owner || wsRole == WorkspaceRole.Admin;
        }

        private async Task<bool> CanViewProjectAsync(Guid userId, Guid projectId)
        {
            var projRole = await _roleRepo.GetProjectRoleAsync(userId, projectId);
            var workspaceId = await _roleRepo.GetWorkspaceIdFromProjectAsync(projectId);
            var wsRole = await _roleRepo.GetWorkspaceRoleAsync(userId, workspaceId);

            return projRole != null || wsRole == WorkspaceRole.Owner || wsRole == WorkspaceRole.Admin;
        }

        private async Task<bool> CanDeleteProjectAsync(Guid userId, Guid projectId)
        {
            var projRole = await _roleRepo.GetProjectRoleAsync(userId, projectId);
            return projRole == ProjectRole.Owner;
        }

        // Task Logic
        private async Task<bool> CanEditTaskAsync(Guid userId, Guid taskId)
        {
            var projectId = await _roleRepo.GetProjectIdFromTaskAsync(taskId);
            return await CanEditProjectAsync(userId, projectId);
        }

        private async Task<bool> CanUpdateTaskStatusAsync(Guid userId, Guid taskId)
        {
            if (await _roleRepo.IsAssigneeAsync(userId, taskId))
                return true;

            var projectId = await _roleRepo.GetProjectIdFromTaskAsync(taskId);
            return await CanEditProjectAsync(userId, projectId);
        }

        private async Task<bool> CanLogTimeAsync(Guid userId, Guid taskId)
        {
            return await _roleRepo.IsAssigneeAsync(userId, taskId);
        }

        private async Task<bool> CanViewTaskAsync(Guid userId, Guid taskId)
        {
            if (await _roleRepo.IsAssigneeAsync(userId, taskId))
                return true;

            var projectId = await _roleRepo.GetProjectIdFromTaskAsync(taskId);
            return await CanViewProjectAsync(userId, projectId);
        }

        private async Task<bool> CanDeleteTaskAsync(Guid userId, Guid taskId)
        {
            if (await _roleRepo.IsTaskCreatorAsync(userId, taskId))
                return true;

            var projectId = await _roleRepo.GetProjectIdFromTaskAsync(taskId);
            return await CanDeleteProjectAsync(userId, projectId);
        }

        // Bulk Permission Resolvers
        public async Task<List<string>> GetPermissionsForWorkspaceAsync(Guid userId, Guid workspaceId)
        {
            var result = new List<string>();
            foreach (WorkspacePermission perm in Enum.GetValues<WorkspacePermission>())
            {
                if (await HasPermissionAsync(userId, perm, workspaceId))
                    result.Add(perm.ToString());
            }
            return result;
        }

        public async Task<List<string>> GetPermissionsForProjectAsync(Guid userId, Guid projectId)
        {
            var result = new List<string>();
            foreach (ProjectPermission perm in Enum.GetValues<ProjectPermission>())
            {
                if (await HasPermissionAsync(userId, perm, projectId))
                    result.Add(perm.ToString());
            }
            return result;
        }

        public async Task<List<string>> GetPermissionsForTaskAsync(Guid userId, Guid taskId)
        {
            var result = new List<string>();
            foreach (TaskPermission perm in Enum.GetValues<TaskPermission>())
            {
                if (await HasPermissionAsync(userId, perm, taskId))
                    result.Add(perm.ToString());
            }
            return result;
        }
    }
}
