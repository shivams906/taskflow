using TaskFlowAPI.Interfaces;
using TaskFlowAPI.Models.Enum;
using TaskFlowAPI.Models.Enums;

namespace TaskFlowAPI.Services
{
    public class RoleAccessService : IRoleAccessService
    {
        private readonly IUserRoleRepository _roleRepo;

        public RoleAccessService(IUserRoleRepository roleRepo)
        {
            _roleRepo = roleRepo;
        }

        public async Task<bool> CanEditWorkspaceAsync(Guid userId, Guid workspaceId)
        {
            var role = await _roleRepo.GetWorkspaceRoleAsync(userId, workspaceId);
            return role == WorkspaceRole.Owner || role == WorkspaceRole.Admin;
        }

        public async Task<bool> CanViewWorkspaceAsync(Guid userId, Guid workspaceId)
        {
            var role = await _roleRepo.GetWorkspaceRoleAsync(userId, workspaceId);
            return role != null;
        }

        public async Task<bool> CanDeleteWorkspaceAsync(Guid userId, Guid workspaceId)
        {
            var role = await _roleRepo.GetWorkspaceRoleAsync(userId, workspaceId);
            return role == WorkspaceRole.Owner;
        }

        public async Task<bool> CanCreateProjectAsync(Guid userId, Guid workspaceId)
        {
            var role = await _roleRepo.GetWorkspaceRoleAsync(userId, workspaceId);
            return role == WorkspaceRole.Owner || role == WorkspaceRole.Admin;
        }

        public async Task<bool> CanEditProjectAsync(Guid userId, Guid projectId)
        {
            var projRole = await _roleRepo.GetProjectRoleAsync(userId, projectId);
            var workspaceId = await _roleRepo.GetWorkspaceIdFromProjectAsync(projectId);
            var wsRole = await _roleRepo.GetWorkspaceRoleAsync(userId, workspaceId);

            return projRole == ProjectRole.Owner || projRole == ProjectRole.Admin ||
                   wsRole == WorkspaceRole.Owner || wsRole == WorkspaceRole.Admin;
        }

        public async Task<bool> CanViewProjectAsync(Guid userId, Guid projectId)
        {
            var projRole = await _roleRepo.GetProjectRoleAsync(userId, projectId);
            var workspaceId = await _roleRepo.GetWorkspaceIdFromProjectAsync(projectId);
            var wsRole = await _roleRepo.GetWorkspaceRoleAsync(userId, workspaceId);

            return projRole != null || wsRole == WorkspaceRole.Owner || wsRole == WorkspaceRole.Admin;
        }

        public async Task<bool> CanDeleteProjectAsync(Guid userId, Guid projectId)
        {
            var role = await _roleRepo.GetProjectRoleAsync(userId, projectId);
            return role == ProjectRole.Owner;
        }

        public async Task<bool> CanCreateTaskAsync(Guid userId, Guid projectId)
        {
            return await CanEditProjectAsync(userId, projectId);
        }

        public async Task<bool> CanEditTaskAsync(Guid userId, Guid taskId)
        {
            var projectId = await _roleRepo.GetProjectIdFromTaskAsync(taskId);
            return await CanEditProjectAsync(userId, projectId);
        }

        public async Task<bool> CanUpdateTaskStatusAsync(Guid userId, Guid taskId)
        {
            if (await _roleRepo.IsAssigneeAsync(userId, taskId))
                return true;

            var projectId = await _roleRepo.GetProjectIdFromTaskAsync(taskId);
            return await CanEditProjectAsync(userId, projectId);
        }

        public async Task<bool> CanLogTimeAsync(Guid userId, Guid taskId)
        {
            return await _roleRepo.IsAssigneeAsync(userId, taskId);
        }

        public async Task<bool> CanViewTaskAsync(Guid userId, Guid taskId)
        {
            if (await _roleRepo.IsAssigneeAsync(userId, taskId))
                return true;

            var projectId = await _roleRepo.GetProjectIdFromTaskAsync(taskId);
            return await CanViewProjectAsync(userId, projectId);
        }

        public async Task<bool> CanDeleteTaskAsync(Guid userId, Guid taskId)
        {
            if (await _roleRepo.IsTaskCreatorAsync(userId, taskId))
                return true;

            var projectId = await _roleRepo.GetProjectIdFromTaskAsync(taskId);
            return await CanDeleteProjectAsync(userId, projectId);
        }
    }
}
