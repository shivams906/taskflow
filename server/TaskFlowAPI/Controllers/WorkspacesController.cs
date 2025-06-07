using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlowAPI.DTOs;
using TaskFlowAPI.Interfaces;
using TaskFlowAPI.Models.Enum;

namespace TaskFlowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkspacesController : ControllerBase
    {
        private readonly IWorkspaceService _workspaceService;
        private readonly ICurrentSessionProvider _currentSessionProvider;
        private readonly IRoleAccessService _roleAccessService;

        public WorkspacesController(IWorkspaceService workspaceService, ICurrentSessionProvider currentSessionProvider, IRoleAccessService roleAccessService)
        {
            _workspaceService = workspaceService;
            _currentSessionProvider = currentSessionProvider;
            _roleAccessService = roleAccessService;
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
            if (!await _roleAccessService.HasPermissionAsync(userId, WorkspacePermission.ViewWorkspace, id))
                throw new UnauthorizedAccessException("You do not have access");
            var workspace = await _workspaceService.GetWorkspaceByIdAsync(id, userId);
            return Ok(workspace);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWorkspaceDto updatedWorkspace)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            if (!await _roleAccessService.HasPermissionAsync(userId, WorkspacePermission.ManageWorkspace, id))
                throw new UnauthorizedAccessException("You do not have access");
            var workspace = await _workspaceService.UpdateWorkspaceAsync(id, updatedWorkspace, userId);
            return Ok(workspace);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWorkspaceDto newWorkspace)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            var workspace = await _workspaceService.CreateWorkspaceAsync(newWorkspace, userId);
            return CreatedAtAction("GetById", new { id = workspace.Id }, workspace);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            if (!await _roleAccessService.HasPermissionAsync(userId, WorkspacePermission.DeleteWorkspace, id))
                throw new UnauthorizedAccessException("You do not have access");
            await _workspaceService.DeleteWorkspaceAsync(id, userId);
            return Ok("Workspace deleted.");
        }

        [HttpPost("join")]
        public async Task<IActionResult> Join([FromBody] JoinWorkspaceDto dto)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            var workspace = await _workspaceService.JoinWorkspaceAsync(dto.InviteCode, userId);
            return Ok(workspace);
        }

        [HttpGet("{workspaceId}/projects")]
        public async Task<IActionResult> GetProjects(Guid workspaceId)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            if (!await _roleAccessService.HasPermissionAsync(userId, WorkspacePermission.ViewWorkspace, workspaceId))
                throw new UnauthorizedAccessException("You do not have access");
            var projects = await _workspaceService.GetProjectsForWorkspaceAsync(workspaceId, userId);
            return Ok(projects);
        }

        [HttpPost("{workspaceId}/projects")]
        public async Task<IActionResult> CreateProject(Guid workspaceId, [FromBody] CreateProjectDto dto)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            if (!await _roleAccessService.HasPermissionAsync(userId, WorkspacePermission.ManageWorkspace, workspaceId))
                throw new UnauthorizedAccessException("You do not have access");
            var project = await _workspaceService.CreateProjectAsync(workspaceId, dto, userId);
            return Ok(project);
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
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            if (!await _roleAccessService.HasPermissionAsync(userId, WorkspacePermission.ViewWorkspace, workspaceId))
                throw new UnauthorizedAccessException("You do not have access");
            var users = await _workspaceService.GetWorkspaceUsersAsync(workspaceId);
            return Ok(users);
        }
    }
}
