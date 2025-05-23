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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkspacesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentSessionProvider _currentSessionProvider;

        public WorkspacesController(AppDbContext context, IMapper mapper, ICurrentSessionProvider currentSessionProvider)
        {
            _context = context;
            _mapper = mapper;
            _currentSessionProvider = currentSessionProvider;
        }

        // GET: api/Workspaces
        [HttpGet]
        public async Task<ActionResult> GetMyWorkspaces()
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");

            var workspaces = await _context.Workspaces
                .Where(w => w.WorkspaceUsers.Any(wu => wu.UserId == userId))
                .Select(p => _mapper.Map<WorkspaceDto>(p))
                .ToListAsync();

            return Ok(workspaces);
        }

        // GET: api/Workspaces/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Workspace>> GetWorkspace(Guid id)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");

            var workspace = await _context.Workspaces
                .FirstOrDefaultAsync(w =>
                    w.Id == id &&
                    w.WorkspaceUsers.Any(wu => wu.UserId == userId));

            if (workspace == null)
                return NotFound("Workspace not found or you do not have access.");

            return Ok(_mapper.Map<WorkspaceDto>(workspace));
        }

        // Patch: api/Workspaces/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchWorkspace(Guid id, [FromBody] UpdateWorkspaceDto updatedWorkspace)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");

            var workspace = await _context.Workspaces
                .FirstOrDefaultAsync(w => w.Id == id && w.WorkspaceUsers.Any(wu => wu.UserId == userId && (wu.Role == WorkspaceRole.Owner || wu.Role == WorkspaceRole.Admin)));
            if (workspace == null)
                return Forbid("Only the workspace owner or admin can update this project.");

            workspace.Name = updatedWorkspace.Name;
            workspace.UpdatedById = userId;
            workspace.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<WorkspaceDto>(workspace));
        }

        // POST: api/Workspaces
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostWorkspace([FromBody] CreateWorkspaceDto newWorkspace)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");

            var inviteCode = Guid.NewGuid().ToString("N").Substring(0, 8);

            var workspace = _mapper.Map<Workspace>(newWorkspace);
            workspace.Id = Guid.NewGuid();
            workspace.InviteCode = inviteCode;
            workspace.CreatedById = userId;
            workspace.CreatedAtUtc = DateTime.UtcNow;

            _context.Workspaces.Add(workspace);

            // Add creator as Admin
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
            return CreatedAtAction("GetWorkspace", new { id = workspace.Id }, _mapper.Map<WorkspaceDto>(workspace));
        }

        // DELETE: api/Workspaces/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkspace(Guid id)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");

            var workspace = await _context.Workspaces
                .FirstOrDefaultAsync(w => w.Id == id && w.WorkspaceUsers.Any(wu => wu.UserId == userId && wu.Role == WorkspaceRole.Owner));

            if (workspace == null)
                return Forbid("Only the owner can delete this workspace.");

            _context.Workspaces.Remove(workspace);

            await _context.SaveChangesAsync();

            return Ok("Workspace deleted.");
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinWorkspace([FromBody] JoinWorkspaceDto dto)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");

            var workspace = await _context.Workspaces
                .FirstOrDefaultAsync(w => w.InviteCode == dto.InviteCode);

            if (workspace == null)
                return NotFound("Invalid invite code");

            var alreadyMember = await _context.WorkspaceUsers
                .AnyAsync(wu => wu.WorkspaceId == workspace.Id && wu.UserId == userId);

            if (alreadyMember)
                return BadRequest("Already in this workspace.");

            _context.WorkspaceUsers.Add(new WorkspaceUser
            {
                WorkspaceId = workspace.Id,
                UserId = userId,
                Role = WorkspaceRole.Member
            });

            await _context.SaveChangesAsync();

            return Ok("Joined workspace successfully.");
        }

        [HttpGet("{workspaceId}/projects")]
        public async Task<IActionResult> GetProjectsForWorkspace(Guid workspaceId)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");

            var projects = await _context.Projects
                .Where(p => (p.CreatedById == userId ||
                            p.ProjectUsers.Any(pu => pu.UserId == userId)) && p.WorkspaceId == workspaceId)
                .Include(p => p.ProjectUsers)!
                    .ThenInclude(pu => pu.User)
                .Include(p => p.CreatedBy)
                .Select(p => _mapper.Map<ProjectDto>(p))
                .ToListAsync();

            return Ok(projects);
        }

        [HttpPost("{workspaceId}/projects")]
        public async Task<IActionResult> CreateProject(Guid workspaceId, [FromBody] CreateProjectDto dto)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");

            var workspace = await _context.Workspaces.FirstOrDefaultAsync(w => w.Id == workspaceId);
            if (workspace == null)
            {
                return BadRequest("Workspace does not exist");
            }

            var hasAccess = _context.WorkspaceUsers.FirstOrDefaultAsync(wu => wu.WorkspaceId == workspaceId && wu.UserId == userId && (wu.Role == WorkspaceRole.Owner || wu.Role == WorkspaceRole.Admin));
            if (hasAccess == null)
            {
                return Forbid("You do not have access");
            }

            var project = _mapper.Map<Project>(dto);
            project.Id = Guid.NewGuid();
            project.CreatedById = userId;
            project.CreatedAtUtc = DateTime.UtcNow;
            project.WorkspaceId = workspaceId;

            _context.Projects.Add(project);

            // Add creator as Admin
            var projectUser = new ProjectUser
            {
                ProjectId = project.Id,
                UserId = userId,
                Role = ProjectRole.Admin,
                CreatedById = userId,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.ProjectUsers.Add(projectUser);

            await _context.SaveChangesAsync();
            await _context.Entry(project).Reference(p => p.CreatedBy).LoadAsync();
            var projectDto = _mapper.Map<ProjectDto>(project);
            return Ok(projectDto);
        }

        [HttpGet("{workspaceId}/my-tasks")]
        public async Task<IActionResult> GetMyTasks(Guid workspaceId)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");

            var tasks = await _context.Tasks
                .Include(t => t.Project)
                .Where(t => t.AssignedToId == userId && t.Project.WorkspaceId == workspaceId)
                .Select(t => _mapper.Map<TaskDto>(t))
                .ToListAsync();

            //var dtos = _mapper.Map<List<TaskDto>>(tasks);

            return Ok(tasks);
        }
    }
}
