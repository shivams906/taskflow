using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlowAPI.DTOs;
using TaskFlowAPI.Interfaces;
using TaskFlowAPI.Models.Enum;

namespace TaskFlowAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly ICurrentSessionProvider _currentSessionProvider;
        private readonly IRoleAccessService _roleAccessService;

        public ProjectsController(IProjectService projectService, ICurrentSessionProvider currentSessionProvider, IRoleAccessService roleAccessService)
        {
            _projectService = projectService;
            _currentSessionProvider = currentSessionProvider;
            _roleAccessService = roleAccessService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyProjects()
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            var projects = await _projectService.GetMyProjectsAsync(userId);
            return Ok(projects);
        }

        [HttpPost("{id}/users")]
        public async Task<IActionResult> AddProjectUser(Guid id, [FromBody] AddProjectUserDto dto)
        {
            var currentUserId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            if (!await _roleAccessService.HasPermissionAsync(currentUserId, ProjectPermission.ManageProject, id))
                throw new UnauthorizedAccessException("You do not have access");
            var success = await _projectService.AddProjectUserAsync(id, dto, currentUserId);
            return Ok("User added to project successfully.");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            if (!await _roleAccessService.HasPermissionAsync(userId, ProjectPermission.ViewProject, id))
                throw new UnauthorizedAccessException("You do not have access");
            var project = await _projectService.GetProjectByIdAsync(id, userId);

            return Ok(project);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateProjectDto updated)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            if (!await _roleAccessService.HasPermissionAsync(userId, ProjectPermission.ManageProject, id))
                throw new UnauthorizedAccessException("You do not have access");
            var updatedProject = await _projectService.UpdateProjectAsync(id, updated, userId);
            return Ok(updatedProject);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            if (!await _roleAccessService.HasPermissionAsync(userId, ProjectPermission.DeleteProject, id))
                throw new UnauthorizedAccessException("You do not have access");
            var deleted = await _projectService.DeleteProjectAsync(id, userId);
            return Ok("Project deleted.");
        }

        [HttpGet("{id}/users")]
        public async Task<IActionResult> GetProjectUsers(Guid id)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            if (!await _roleAccessService.HasPermissionAsync(userId, ProjectPermission.ViewProject, id))
                throw new UnauthorizedAccessException("You do not have access");
            var users = await _projectService.GetProjectUsersAsync(id, userId);
            return Ok(users);
        }
    }
}
