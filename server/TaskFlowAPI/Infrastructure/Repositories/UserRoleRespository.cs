using Microsoft.EntityFrameworkCore;
using TaskFlowAPI.Data;
using TaskFlowAPI.Interfaces;
using TaskFlowAPI.Models.Enum;
using TaskFlowAPI.Models.Enums;

namespace TaskFlowAPI.Infrastructure.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly AppDbContext _db;

        public UserRoleRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<WorkspaceRole?> GetWorkspaceRoleAsync(Guid userId, Guid workspaceId)
        {
            return await _db.WorkspaceUsers
                .Where(wu => wu.UserId == userId && wu.WorkspaceId == workspaceId)
                .Select(wu => (WorkspaceRole?)wu.Role)
                .FirstOrDefaultAsync();
        }

        public async Task<ProjectRole?> GetProjectRoleAsync(Guid userId, Guid projectId)
        {
            return await _db.ProjectUsers
                .Where(pu => pu.UserId == userId && pu.ProjectId == projectId)
                .Select(pu => (ProjectRole?)pu.Role)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsAssigneeAsync(Guid userId, Guid taskId)
        {
            return await _db.Tasks
                .AnyAsync(t => t.Id == taskId && t.AssignedToId == userId);
        }

        public async Task<bool> IsTaskCreatorAsync(Guid userId, Guid taskId)
        {
            return await _db.Tasks
                .AnyAsync(t => t.Id == taskId && t.CreatedById == userId);
        }

        public async Task<Guid> GetProjectIdFromTaskAsync(Guid taskId)
        {
            return await _db.Tasks
                .Where(t => t.Id == taskId)
                .Select(t => (Guid)t.ProjectId)
                .FirstOrDefaultAsync();
        }

        public async Task<Guid> GetWorkspaceIdFromProjectAsync(Guid projectId)
        {
            return await _db.Projects
                .Where(p => p.Id == projectId)
                .Select(p => (Guid)p.WorkspaceId)
                .FirstOrDefaultAsync();
        }
    }
}
