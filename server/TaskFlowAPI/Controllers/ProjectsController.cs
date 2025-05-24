using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowAPI.Data;
using TaskFlowAPI.DTOs;
using TaskFlowAPI.Interfaces;
using TaskFlowAPI.Models;
using TaskFlowAPI.Models.Enum;
using TaskFlowAPI.Models.Enums;

namespace TaskFlowAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentSessionProvider _currentSessionProvider;

        public ProjectsController(AppDbContext context, IMapper mapper, ICurrentSessionProvider currentSessionProvider)
        {
            _context = context;
            _mapper = mapper;
            _currentSessionProvider = currentSessionProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyProjects()
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");

            var projects = await _context.Projects
                .Where(p => p.CreatedById == userId ||
                            p.ProjectUsers.Any(pu => pu.UserId == userId && pu.Role == ProjectRole.Admin))
                .Include(p => p.ProjectUsers)!
                    .ThenInclude(pu => pu.User)
                .Include(p => p.CreatedBy)
                .Select(p => _mapper.Map<ProjectDto>(p))
                .ToListAsync();

            return Ok(projects);
        }

        [HttpPost("{id}/users")]
        public async Task<IActionResult> AddProjectUser(Guid id, [FromBody] AddProjectUserDto dto)
        {
            var currentUserId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");

            var project = await _context.Projects
                .Include(p => p.Workspace)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
                return NotFound("Project not found.");

            // Check if current user is a ProjectAdmin
            bool isProjectAdmin = await _context.ProjectUsers
                .AnyAsync(pu => pu.ProjectId == id && pu.UserId == currentUserId && (pu.Role == ProjectRole.Admin || pu.Role == ProjectRole.Owner));

            // Check if current user is a Workspace Admin
            bool isWorkspaceAdmin = await _context.WorkspaceUsers
                .AnyAsync(wu => wu.WorkspaceId == project.WorkspaceId && wu.UserId == currentUserId && (wu.Role == WorkspaceRole.Admin || wu.Role == WorkspaceRole.Owner));

            if (!(isProjectAdmin || isWorkspaceAdmin))
                return Forbid("Only a project admin or workspace admin can add members.");

            // Check if user is already in the project
            bool alreadyAdded = await _context.ProjectUsers
                .AnyAsync(pu => pu.ProjectId == id && pu.UserId == dto.UserId);

            if (alreadyAdded)
                return BadRequest("User is already a member of this project.");

            var projectUser = _mapper.Map<ProjectUser>(dto);
            projectUser.ProjectId = id;
            projectUser.CreatedById = currentUserId;
            projectUser.CreatedAtUtc = DateTime.UtcNow;

            _context.ProjectUsers.Add(projectUser);
            await _context.SaveChangesAsync();

            return Ok("User added to project successfully.");
        }


        //[HttpDelete("remove-admin")]
        //public async Task<IActionResult> RemoveAdmin([FromBody] AddProjectAdminDto dto)
        //{
        //    var currentUserId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");

        //    // Only creator can remove admin
        //    var project = await _context.Projects
        //        .FirstOrDefaultAsync(p => p.Id == dto.ProjectId && p.CreatedById == currentUserId);

        //    if (project == null)
        //        return Forbid("Only the project creator can remove admins.");

        //    var projectUser = await _context.ProjectUsers
        //        .FirstOrDefaultAsync(pu => pu.ProjectId == dto.ProjectId && pu.UserId == dto.UserId && pu.Role == ProjectRole.Admin);

        //    if (projectUser == null)
        //        return NotFound("User is not an admin of this project.");

        //    _context.ProjectUsers.Remove(projectUser);
        //    await _context.SaveChangesAsync();

        //    return Ok("Admin removed.");
        //}


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");

            var project = await _context.Projects
                .Include(p => p.ProjectUsers)!
                    .ThenInclude(pu => pu.User)
                .Include(p => p.Tasks)
                .Include(p => p.CreatedBy)
                .FirstOrDefaultAsync(p =>
                    p.Id == id &&
                    (p.CreatedById == userId || p.ProjectUsers.Any(pu => pu.UserId == userId && pu.Role == ProjectRole.Admin)));

            if (project == null)
                return NotFound("Project not found or you do not have access.");

            var projectDto = _mapper.Map<ProjectDto>(project);

            return Ok(projectDto);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateProject(Guid id, [FromBody] CreateProjectDto updated)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");

            var project = await _context.Projects
                .Include(p => p.CreatedBy)
                .FirstOrDefaultAsync(p => p.Id == id && p.CreatedById == userId);

            if (project == null)
                return Forbid("Only the project creator can update this project.");

            project.Title = updated.Title;
            project.Description = updated.Description;
            project.UpdatedById = userId;
            project.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var projectDto = _mapper.Map<ProjectDto>(project);

            return Ok(projectDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");

            var project = await _context.Projects
                .Include(p => p.Tasks)
                .Include(p => p.ProjectUsers)
                .FirstOrDefaultAsync(p => p.Id == id && p.CreatedById == userId);

            if (project == null)
                return Forbid("Only the creator can delete this project.");

            // Remove related tasks and users first
            _context.Tasks.RemoveRange(project.Tasks);
            _context.ProjectUsers.RemoveRange(project.ProjectUsers);
            _context.Projects.Remove(project);

            await _context.SaveChangesAsync();

            return Ok("Project deleted.");
        }

        [HttpGet("{id}/users")]
        public async Task<IActionResult> GetProjectUsers(Guid id)
        {
            var users = await _context.ProjectUsers
                .Include(pu => pu.User)
                .Where(pu => pu.ProjectId == id)
                .Select(pu => _mapper.Map<ProjectUserDto>(pu))
                .ToListAsync();
            return Ok(users);
        }
    }
}
