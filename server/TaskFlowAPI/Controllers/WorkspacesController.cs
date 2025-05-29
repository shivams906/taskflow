using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlowAPI.DTOs;
using TaskFlowAPI.Interfaces;

namespace TaskFlowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkspacesController : ControllerBase
    {
        private readonly IWorkspaceService _workspaceService;
        private readonly ICurrentSessionProvider _currentSessionProvider;

        public WorkspacesController(IWorkspaceService workspaceService, ICurrentSessionProvider currentSessionProvider)
        {
            _workspaceService = workspaceService;
            _currentSessionProvider = currentSessionProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyWorkspaces()
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            var workspaces = await _workspaceService.GetMyWorkspacesAsync(userId);
            return Ok(workspaces);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            var workspace = await _workspaceService.GetWorkspaceByIdAsync(id, userId);

            if (workspace == null)
                return NotFound("Workspace not found or you do not have access.");

            return Ok(workspace);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePartial(Guid id, [FromBody] UpdateWorkspaceDto updatedWorkspace)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");

            try
            {
                var workspace = await _workspaceService.UpdateWorkspaceAsync(id, updatedWorkspace, userId);
                return Ok(workspace);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWorkspaceDto newWorkspace)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            var workspace = await _workspaceService.CreateWorkspaceAsync(newWorkspace, userId);
            return CreatedAtAction("GetWorkspace", new { id = workspace.Id }, workspace);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");

            try
            {
                await _workspaceService.DeleteWorkspaceAsync(id, userId);
                return Ok("Workspace deleted.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpPost("join")]
        public async Task<IActionResult> Join([FromBody] JoinWorkspaceDto dto)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");

            try
            {
                var workspace = await _workspaceService.JoinWorkspaceAsync(dto.InviteCode, userId);
                return Ok(workspace);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{workspaceId}/projects")]
        public async Task<IActionResult> GetProjects(Guid workspaceId)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            var projects = await _workspaceService.GetProjectsForWorkspaceAsync(workspaceId, userId);
            return Ok(projects);
        }

        [HttpPost("{workspaceId}/projects")]
        public async Task<IActionResult> CreateProject(Guid workspaceId, [FromBody] CreateProjectDto dto)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");

            try
            {
                var project = await _workspaceService.CreateProjectAsync(workspaceId, dto, userId);
                return Ok(project);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpGet("{workspaceId}/my-tasks")]
        public async Task<IActionResult> GetMyTasks(Guid workspaceId)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            var tasks = await _workspaceService.GetMyTasksAsync(workspaceId, userId);
            return Ok(tasks);
        }

        [HttpGet("{workspaceId}/users")]
        public async Task<IActionResult> GetWorkspaceUsers(Guid workspaceId)
        {
            var users = await _workspaceService.GetWorkspaceUsersAsync(workspaceId);
            return Ok(users);
        }
    }
}
