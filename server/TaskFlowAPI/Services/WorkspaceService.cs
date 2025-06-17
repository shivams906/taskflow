using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskFlowAPI.Data;
using TaskFlowAPI.DTOs;
using TaskFlowAPI.Interfaces;
using TaskFlowAPI.Models;
using TaskFlowAPI.Models.Enum;

namespace TaskFlowAPI.Services;
public class WorkspaceService : IWorkspaceService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IChangeLogService _changeLogService;
    private readonly IRoleAccessService _roleAccessService;

    public WorkspaceService(AppDbContext context, IMapper mapper, IChangeLogService changeLogService, IRoleAccessService roleAccessService)
    {
        _context = context;
        _mapper = mapper;
        _changeLogService = changeLogService;
        _roleAccessService = roleAccessService;
    }

    public async Task<List<WorkspaceDto>> GetMyWorkspacesAsync(Guid userId)
    {
        var workspaceEntities = await _context.Workspaces
            .AsNoTracking()
            .Include(w => w.WorkspaceUsers)
            .ThenInclude(wu => wu.User)
            .Where(w => w.WorkspaceUsers.Any(wu => wu.UserId == userId))
            .ToListAsync();

        var workspaceDtos = _mapper.Map<List<WorkspaceDto>>(workspaceEntities);

        foreach (var dto in workspaceDtos)
        {
            dto.Permissions = await _roleAccessService.GetPermissionsForWorkspaceAsync(userId, dto.Id);
        }

        return workspaceDtos;
    }

    public async Task<WorkspaceDto?> GetWorkspaceByIdAsync(Guid workspaceId, Guid userId)
    {
        var workspace = await _context.Workspaces
            .AsNoTracking()
            .Include(w => w.WorkspaceUsers)
            .ThenInclude(wu => wu.User)
            .Include(w => w.CreatedBy)
            .Include(w => w.UpdatedBy)
            .FirstOrDefaultAsync(w => w.Id == workspaceId) ?? throw new KeyNotFoundException("Workspace not found");
        var workspaceDto = _mapper.Map<WorkspaceDto>(workspace);
        workspaceDto.Permissions = await _roleAccessService.GetPermissionsForWorkspaceAsync(userId, workspaceId);
        return workspaceDto;
    }

    public async Task<WorkspaceDto> UpdateWorkspaceAsync(Guid workspaceId, UpdateWorkspaceDto updateDto, Guid userId)
    {
        var workspace = await _context.Workspaces
            .FirstOrDefaultAsync(w => w.Id == workspaceId) ?? throw new KeyNotFoundException("Workspace not found");
        var changes = new List<string>();
        if (workspace.Name != updateDto.Name)
        {
            changes.Add($"Name changed from {workspace.Name} to {updateDto.Name}");
            workspace.Name = updateDto.Name;
        }
        workspace.UpdatedById = userId;
        workspace.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        foreach (var summary in changes)
            await _changeLogService.LogChange("Workspace", workspaceId, userId, summary);

        var workspaceDto = _mapper.Map<WorkspaceDto>(workspace);
        workspaceDto.Permissions = await _roleAccessService.GetPermissionsForWorkspaceAsync(userId, workspaceId);
        return workspaceDto;
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

        await _changeLogService.LogChange("Workspace", workspace.Id, userId, "Workspace created");

        var workspaceDto = _mapper.Map<WorkspaceDto>(workspace);
        workspaceDto.Permissions = await _roleAccessService.GetPermissionsForWorkspaceAsync(userId, workspace.Id);
        return workspaceDto;
    }

    public async Task DeleteWorkspaceAsync(Guid workspaceId, Guid userId)
    {
        var workspace = await _context.Workspaces
            .FirstOrDefaultAsync(w => w.Id == workspaceId) ?? throw new KeyNotFoundException("Workspace not found");
        _context.Workspaces.Remove(workspace);
        await _context.SaveChangesAsync();
        await _changeLogService.LogChange("Workspace", workspace.Id, userId, "Workspace deleted");
    }

    public async Task<WorkspaceDto> JoinWorkspaceAsync(string inviteCode, Guid userId)
    {
        var workspace = await _context.Workspaces
            .FirstOrDefaultAsync(w => w.InviteCode == inviteCode) ?? throw new KeyNotFoundException("Invalid invite code");
        var alreadyMember = await _context.WorkspaceUsers
            .AnyAsync(wu => wu.WorkspaceId == workspace.Id && wu.UserId == userId);

        if (alreadyMember) throw new InvalidOperationException("Already in this workspace.");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new KeyNotFoundException("Invalid user");

        _context.WorkspaceUsers.Add(new WorkspaceUser
        {
            WorkspaceId = workspace.Id,
            UserId = userId,
            Role = WorkspaceRole.Member
        });

        await _context.SaveChangesAsync();

        await _changeLogService.LogChange("Workspace", workspace.Id, userId, $"{user.Username} joined");

        var workspaceDto = _mapper.Map<WorkspaceDto>(workspace);
        workspaceDto.Permissions = await _roleAccessService.GetPermissionsForWorkspaceAsync(userId, workspace.Id);
        return workspaceDto;
    }

    public async Task<List<ProjectDto>> GetProjectsForWorkspaceAsync(Guid workspaceId, Guid userId)
    {
        var projects = await _context.Projects
            .AsNoTracking()
            .Where(p => p.WorkspaceId == workspaceId && (p.CreatedById == userId || p.ProjectUsers.Any(pu => pu.UserId == userId)))
            .Include(p => p.ProjectUsers)
                .ThenInclude(pu => pu.User)
            .Include(p => p.CreatedBy)
            .ToListAsync();
        var projectDtos = _mapper.Map<List<ProjectDto>>(projects);
        foreach (var dto in projectDtos)
        {
            dto.Permissions = await _roleAccessService.GetPermissionsForProjectAsync(userId, dto.Id);
        }
        return projectDtos;
    }

    public async Task<ProjectDto> CreateProjectAsync(Guid workspaceId, CreateProjectDto dto, Guid userId)
    {
        var workspace = await _context.Workspaces.FirstOrDefaultAsync(w => w.Id == workspaceId) ?? throw new KeyNotFoundException("Workspace does not exist");
        var project = _mapper.Map<Project>(dto);
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

        await _changeLogService.LogChange("Workspace", workspaceId, userId, $"Project {dto.Title} created");
        await _changeLogService.LogChange("Project", project.Id, userId, $"Project created");

        var projectDto = _mapper.Map<ProjectDto>(project);
        projectDto.Permissions = await _roleAccessService.GetPermissionsForProjectAsync(userId, project.Id);
        return projectDto;
    }

    public async Task<List<TaskDto>> GetMyTasksAsync(Guid workspaceId, Guid userId)
    {
        var tasks = await _context.Tasks
            .AsNoTracking()
            .Include(t => t.Project)
            .Where(t => t.AssignedToId == userId && t.Project.WorkspaceId == workspaceId)
            .ToListAsync();
        var taskDtos = _mapper.Map<List<TaskDto>>(tasks);
        foreach (var dto in taskDtos)
        {
            dto.Permissions = await _roleAccessService.GetPermissionsForTaskAsync(userId, dto.Id);
        }
        return taskDtos;
    }

    public async Task<List<WorkspaceUserDto>> GetWorkspaceUsersAsync(Guid workspaceId)
    {
        return await _context.WorkspaceUsers
            .AsNoTracking()
            .Include(wu => wu.User)
            .Where(wu => wu.WorkspaceId == workspaceId)
            .Select(wu => _mapper.Map<WorkspaceUserDto>(wu))
            .ToListAsync();
    }
}
