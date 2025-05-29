using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlowAPI.DTOs;
using TaskFlowAPI.Interfaces;

namespace TaskFlowAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly ICurrentSessionProvider _currentSessionProvider;

        public ProjectsController(IProjectService projectService, ICurrentSessionProvider currentSessionProvider)
        {
            _projectService = projectService;
            _currentSessionProvider = currentSessionProvider;
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

            try
            {
                var success = await _projectService.AddProjectUserAsync(id, dto, currentUserId);
                if (!success)
                    return Forbid("Only a project admin or workspace admin can add members.");
                return Ok("User added to project successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            var project = await _projectService.GetProjectByIdAsync(id, userId);

            if (project == null)
                return NotFound("Project not found or you do not have access.");

            return Ok(project);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateProjectDto updated)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            var updatedProject = await _projectService.UpdateProjectAsync(id, updated, userId);

            if (updatedProject == null)
                return Forbid("Only the project creator can update this project.");

            return Ok(updatedProject);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            var deleted = await _projectService.DeleteProjectAsync(id, userId);

            if (!deleted)
                return Forbid("Only the creator can delete this project.");

            return Ok("Project deleted.");
        }

        [HttpGet("{id}/users")]
        public async Task<IActionResult> GetProjectUsers(Guid id)
        {
            var users = await _projectService.GetProjectUsersAsync(id);
            return Ok(users);
        }
    }
}
