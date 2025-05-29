using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskFlowAPI.Data;
using TaskFlowAPI.DTOs;
using TaskFlowAPI.Interfaces;
using TaskFlowAPI.Models;
using TaskFlowAPI.Models.Enum;
using TaskFlowAPI.Models.Enums;

namespace TaskFlowAPI.Services;
public class ProjectService : IProjectService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ProjectService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ProjectDto>> GetMyProjectsAsync(Guid userId)
    {
        var projects = await _context.Projects
            .Where(p => p.CreatedById == userId ||
                        p.ProjectUsers.Any(pu => pu.UserId == userId && pu.Role == ProjectRole.Admin))
            .Include(p => p.ProjectUsers)!
                .ThenInclude(pu => pu.User)
            .Include(p => p.CreatedBy)
            .ToListAsync();

        return _mapper.Map<List<ProjectDto>>(projects);
    }

    public async Task<bool> AddProjectUserAsync(Guid projectId, AddProjectUserDto dto, Guid currentUserId)
    {
        var project = await _context.Projects.Include(p => p.Workspace).FirstOrDefaultAsync(p => p.Id == projectId);
        if (project == null) return false;

        // Check permissions
        bool isProjectAdmin = await _context.ProjectUsers
            .AnyAsync(pu => pu.ProjectId == projectId && pu.UserId == currentUserId && (pu.Role == ProjectRole.Admin || pu.Role == ProjectRole.Owner));

        bool isWorkspaceAdmin = await _context.WorkspaceUsers
            .AnyAsync(wu => wu.WorkspaceId == project.WorkspaceId && wu.UserId == currentUserId && (wu.Role == WorkspaceRole.Admin || wu.Role == WorkspaceRole.Owner));

        if (!(isProjectAdmin || isWorkspaceAdmin)) return false;

        // Check if user already added
        bool alreadyAdded = await _context.ProjectUsers.AnyAsync(pu => pu.ProjectId == projectId && pu.UserId == dto.UserId);
        if (alreadyAdded) throw new InvalidOperationException("User is already a member of this project.");

        var projectUser = _mapper.Map<ProjectUser>(dto);
        projectUser.ProjectId = projectId;
        projectUser.CreatedById = currentUserId;
        projectUser.CreatedAtUtc = DateTime.UtcNow;

        _context.ProjectUsers.Add(projectUser);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<ProjectDto?> GetProjectByIdAsync(Guid projectId, Guid userId)
    {
        var project = await _context.Projects
            .Include(p => p.ProjectUsers)!
                .ThenInclude(pu => pu.User)
            .Include(p => p.Tasks)
            .Include(p => p.CreatedBy)
            .FirstOrDefaultAsync(p =>
                p.Id == projectId &&
                (p.CreatedById == userId || p.ProjectUsers.Any(pu => pu.UserId == userId)));

        return project == null ? null : _mapper.Map<ProjectDto>(project);
    }

    public async Task<ProjectDto?> UpdateProjectAsync(Guid projectId, CreateProjectDto updated, Guid userId)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == projectId && p.CreatedById == userId);

        if (project == null) return null;

        project.Title = updated.Title;
        project.Description = updated.Description;
        project.UpdatedById = userId;
        project.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return _mapper.Map<ProjectDto>(project);
    }

    public async Task<bool> DeleteProjectAsync(Guid projectId, Guid userId)
    {
        var project = await _context.Projects
            .Include(p => p.Tasks)
            .Include(p => p.ProjectUsers)
            .FirstOrDefaultAsync(p => p.Id == projectId && p.CreatedById == userId);

        if (project == null) return false;

        _context.Tasks.RemoveRange(project.Tasks);
        _context.ProjectUsers.RemoveRange(project.ProjectUsers);
        _context.Projects.Remove(project);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<ProjectUserDto>> GetProjectUsersAsync(Guid projectId)
    {
        var users = await _context.ProjectUsers
            .Include(pu => pu.User)
            .Where(pu => pu.ProjectId == projectId)
            .ToListAsync();

        return _mapper.Map<List<ProjectUserDto>>(users);
    }
}
