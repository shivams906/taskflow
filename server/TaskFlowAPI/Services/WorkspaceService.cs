using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskFlowAPI.Data;
using TaskFlowAPI.DTOs;
using TaskFlowAPI.Interfaces;
using TaskFlowAPI.Models;
using TaskFlowAPI.Models.Enum;
using TaskFlowAPI.Models.Enums;

namespace TaskFlowAPI.Services;
public class WorkspaceService : IWorkspaceService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public WorkspaceService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<WorkspaceDto>> GetMyWorkspacesAsync(Guid userId)
    {
        return await _context.Workspaces
            .Where(w => w.WorkspaceUsers.Any(wu => wu.UserId == userId))
            .Select(w => _mapper.Map<WorkspaceDto>(w))
            .ToListAsync();
    }

    public async Task<WorkspaceDto?> GetWorkspaceByIdAsync(Guid workspaceId, Guid userId)
    {
        var workspace = await _context.Workspaces
            .FirstOrDefaultAsync(w => w.Id == workspaceId) ?? throw new KeyNotFoundException("Workspace not found");
        return workspace == null ? null : _mapper.Map<WorkspaceDto>(workspace);
    }

    public async Task<WorkspaceDto> UpdateWorkspaceAsync(Guid workspaceId, UpdateWorkspaceDto updateDto, Guid userId)
    {
        var workspace = await _context.Workspaces
            .FirstOrDefaultAsync(w => w.Id == workspaceId) ?? throw new KeyNotFoundException("Workspace not found");
        workspace.Name = updateDto.Name;
        workspace.UpdatedById = userId;
        workspace.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return _mapper.Map<WorkspaceDto>(workspace);
    }

    public async Task<WorkspaceDto> CreateWorkspaceAsync(CreateWorkspaceDto newWorkspaceDto, Guid userId)
    {
        var inviteCode = Guid.NewGuid().ToString("N").Substring(0, 8);

        var workspace = _mapper.Map<Workspace>(newWorkspaceDto);
        workspace.Id = Guid.NewGuid();
        workspace.InviteCode = inviteCode;
        workspace.CreatedById = userId;
        workspace.CreatedAtUtc = DateTime.UtcNow;

        _context.Workspaces.Add(workspace);

        var workspaceUser = new WorkspaceUser
        {
            WorkspaceId = workspace.Id,
            UserId = userId,
            Role = WorkspaceRole.Owner,
            CreatedById = userId,
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.WorkspaceUsers.Add(workspaceUser);

        await _context.SaveChangesAsync();

        return _mapper.Map<WorkspaceDto>(workspace);
    }

    public async Task DeleteWorkspaceAsync(Guid workspaceId, Guid userId)
    {
        var workspace = await _context.Workspaces
            .FirstOrDefaultAsync(w => w.Id == workspaceId) ?? throw new KeyNotFoundException("Workspace not found");
        _context.Workspaces.Remove(workspace);
        await _context.SaveChangesAsync();
    }

    public async Task<WorkspaceDto> JoinWorkspaceAsync(string inviteCode, Guid userId)
    {
        var workspace = await _context.Workspaces
            .FirstOrDefaultAsync(w => w.InviteCode == inviteCode);

        if (workspace == null) throw new KeyNotFoundException("Invalid invite code");

        var alreadyMember = await _context.WorkspaceUsers
            .AnyAsync(wu => wu.WorkspaceId == workspace.Id && wu.UserId == userId);

        if (alreadyMember) throw new InvalidOperationException("Already in this workspace.");

        _context.WorkspaceUsers.Add(new WorkspaceUser
        {
            WorkspaceId = workspace.Id,
            UserId = userId,
            Role = WorkspaceRole.Member
        });

        await _context.SaveChangesAsync();

        return _mapper.Map<WorkspaceDto>(workspace);
    }

    public async Task<List<ProjectDto>> GetProjectsForWorkspaceAsync(Guid workspaceId, Guid userId)
    {
        return await _context.Projects
            .Where(p => (p.CreatedById == userId || p.ProjectUsers.Any(pu => pu.UserId == userId)) && p.WorkspaceId == workspaceId)
            .Include(p => p.ProjectUsers)!
                .ThenInclude(pu => pu.User)
            .Include(p => p.CreatedBy)
            .Select(p => _mapper.Map<ProjectDto>(p))
            .ToListAsync();
    }

    public async Task<ProjectDto> CreateProjectAsync(Guid workspaceId, CreateProjectDto projectDto, Guid userId)
    {
        var workspace = await _context.Workspaces.FirstOrDefaultAsync(w => w.Id == workspaceId) ?? throw new KeyNotFoundException("Workspace does not exist");
        var project = _mapper.Map<Project>(projectDto);
        project.Id = Guid.NewGuid();
        project.CreatedById = userId;
        project.CreatedAtUtc = DateTime.UtcNow;
        project.WorkspaceId = workspaceId;
        _context.Projects.Add(project);
        var projectUser = new ProjectUser
        {
            ProjectId = project.Id,
            UserId = userId,
            Role = ProjectRole.Owner,
            CreatedById = userId,
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.ProjectUsers.Add(projectUser);

        await _context.SaveChangesAsync();

        await _context.Entry(project).Reference(p => p.CreatedBy).LoadAsync();

        return _mapper.Map<ProjectDto>(project);
    }

    public async Task<List<TaskDto>> GetMyTasksAsync(Guid workspaceId, Guid userId)
    {
        var tasks = await _context.Tasks
            .Include(t => t.Project)
            .Where(t => t.AssignedToId == userId && t.Project.WorkspaceId == workspaceId)
            .Select(t => _mapper.Map<TaskDto>(t))
            .ToListAsync();

        return tasks;
    }

    public async Task<List<WorkspaceUserDto>> GetWorkspaceUsersAsync(Guid workspaceId)
    {
        return await _context.WorkspaceUsers
            .Include(wu => wu.User)
            .Where(wu => wu.WorkspaceId == workspaceId)
            .Select(wu => _mapper.Map<WorkspaceUserDto>(wu))
            .ToListAsync();
    }
}
