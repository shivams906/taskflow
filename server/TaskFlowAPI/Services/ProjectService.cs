using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskFlowAPI.Data;
using TaskFlowAPI.DTOs;
using TaskFlowAPI.Interfaces;
using TaskFlowAPI.Models;
using TaskFlowAPI.Models.Enum;

namespace TaskFlowAPI.Services;
public class ProjectService : IProjectService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IChangeLogService _changeLogService;
    private readonly IRoleAccessService _roleAccessService;

    public ProjectService(AppDbContext context, IMapper mapper, IChangeLogService changeLogService, IRoleAccessService roleAccessService)
    {
        _context = context;
        _mapper = mapper;
        _changeLogService = changeLogService;
        _roleAccessService = roleAccessService;
    }

    public async Task<List<ProjectDto>> GetMyProjectsAsync(Guid userId)
    {
        var projects = await _context.Projects
            .AsNoTracking()
            .Where(p => p.CreatedById == userId ||
                        p.ProjectUsers.Any(pu => pu.UserId == userId && pu.Role == ProjectRole.Admin))
            .Include(p => p.ProjectUsers)!
                .ThenInclude(pu => pu.User)
            .Include(p => p.CreatedBy)
            .ToListAsync();
        var projectDtos = _mapper.Map<List<ProjectDto>>(projects);
        foreach (var project in projectDtos)
        {
            project.Permissions = await _roleAccessService.GetPermissionsForProjectAsync(userId, project.Id);
        }
        return projectDtos;
    }

    public async Task<bool> AddProjectUserAsync(Guid projectId, AddProjectUserDto dto, Guid currentUserId)
    {
        var project = await _context.Projects.Include(p => p.Workspace).FirstOrDefaultAsync(p => p.Id == projectId) ?? throw new KeyNotFoundException("Project not found");

        // Check if user already added
        bool alreadyAdded = await _context.ProjectUsers.AnyAsync(pu => pu.ProjectId == projectId && pu.UserId == dto.UserId);
        if (alreadyAdded) throw new InvalidOperationException("User is already a member of this project.");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId) ?? throw new KeyNotFoundException("Invalid user");

        var projectUser = _mapper.Map<ProjectUser>(dto);
        projectUser.ProjectId = projectId;
        projectUser.CreatedById = currentUserId;
        projectUser.CreatedAtUtc = DateTime.UtcNow;

        _context.ProjectUsers.Add(projectUser);
        await _context.SaveChangesAsync();

        await _changeLogService.LogChange("Project", projectId, currentUserId, $"{user.Name} added as {dto.Role}");

        return true;
    }

    public async Task<ProjectDto?> GetProjectByIdAsync(Guid projectId, Guid userId)
    {

        var project = await _context.Projects
            .AsNoTracking()
            .Include(p => p.ProjectUsers)!
                .ThenInclude(pu => pu.User)
            .Include(p => p.Tasks)
            .Include(p => p.CreatedBy)
            .Include(p => p.UpdatedBy)
            .FirstOrDefaultAsync(p =>
                p.Id == projectId &&
                (p.CreatedById == userId || p.ProjectUsers.Any(pu => pu.UserId == userId))) ?? throw new KeyNotFoundException("Project not found");
        var projectDto = _mapper.Map<ProjectDto>(project);
        projectDto.Permissions = await _roleAccessService.GetPermissionsForProjectAsync(userId, projectId);
        return projectDto;
    }

    public async Task<ProjectDto?> UpdateProjectAsync(Guid projectId, CreateProjectDto updated, Guid userId)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == projectId) ?? throw new KeyNotFoundException("Project not found");
        var changes = new List<string>();
        if (project.Title != updated.Title)
        {
            changes.Add($"Title updated from {project.Title} to {updated.Title}");
            project.Title = updated.Title;
        }
        if (project.Description != updated.Description)
        {
            changes.Add($"Desctiption updated from {project.Description} to {updated.Description}");
            project.Description = updated.Description;
        }
        project.UpdatedById = userId;
        project.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        foreach (var summary in changes)
            await _changeLogService.LogChange("Project", projectId, userId, summary);

        var projectDto = _mapper.Map<ProjectDto>(project);
        projectDto.Permissions = await _roleAccessService.GetPermissionsForProjectAsync(userId, projectId);
        return projectDto;
    }

    public async Task<bool> DeleteProjectAsync(Guid projectId, Guid userId)
    {
        var project = await _context.Projects
            .Include(p => p.Tasks)
            .Include(p => p.ProjectUsers)
            .FirstOrDefaultAsync(p => p.Id == projectId) ?? throw new KeyNotFoundException("Project not found");
        var workspaceId = project.WorkspaceId;
        _context.Tasks.RemoveRange(project.Tasks);
        _context.ProjectUsers.RemoveRange(project.ProjectUsers);
        _context.Projects.Remove(project);

        await _context.SaveChangesAsync();

        await _changeLogService.LogChange("Workspace", workspaceId, userId, $"Project {project.Title} deleted");
        await _changeLogService.LogChange("Project", project.Id, userId, $"Project deleted");

        return true;
    }

    public async Task<List<ProjectUserDto>> GetProjectUsersAsync(Guid projectId, Guid userId)
    {
        var users = await _context.ProjectUsers
            .AsNoTracking()
            .Include(pu => pu.User)
            .Where(pu => pu.ProjectId == projectId)
            .ToListAsync();

        return _mapper.Map<List<ProjectUserDto>>(users);
    }
}
