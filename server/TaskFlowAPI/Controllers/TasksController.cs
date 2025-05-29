using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlowAPI.DTOs;
using TaskFlowAPI.Interfaces;
using TaskFlowAPI.Models.Enums;

namespace TaskFlowAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ICurrentSessionProvider _currentSessionProvider;

        public TasksController(ITaskService taskService, ICurrentSessionProvider currentSessionProvider)
        {
            _taskService = taskService;
            _currentSessionProvider = currentSessionProvider;
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetTasksForProject(Guid projectId)
        {
            try
            {
                var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
                var tasks = await _taskService.GetTasksForProjectAsync(projectId, userId);
                return Ok(tasks);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTaskById(Guid taskId)
        {
            try
            {
                var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
                var task = await _taskService.GetTaskByIdAsync(taskId, userId);
                return Ok(task);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto task)
        {
            try
            {
                var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
                var createdTask = await _taskService.CreateTaskAsync(task, userId);
                return Ok(createdTask);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{taskId}/status")]
        public async Task<IActionResult> UpdateStatus(Guid taskId, [FromBody] UpdateTaskStatusDto dto)
        {
            try
            {
                var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
                await _taskService.UpdateTaskStatusAsync(taskId, dto, userId);
                return Ok("Task status updated.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{taskId}/log-time")]
        public async Task<IActionResult> LogTime(Guid taskId, [FromBody] CreateTimeLogDto dto)
        {
            try
            {
                var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
                await _taskService.LogTimeAsync(taskId, dto, userId);
                return Ok("Time log saved.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{taskId}/logs")]
        public async Task<IActionResult> GetTimeLogsForTask(Guid taskId, [FromQuery] bool onlyMine = false)
        {
            try
            {
                var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
                var logs = await _taskService.GetTimeLogsAsync(taskId, userId, onlyMine);
                return Ok(logs);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] CreateTaskDto updated)
        {
            try
            {
                var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
                var task = await _taskService.UpdateTaskAsync(taskId, updated, userId);
                return Ok(task);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(Guid taskId)
        {
            try
            {
                var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
                await _taskService.DeleteTaskAsync(taskId, userId);
                return Ok("Task deleted.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{taskId}/assign")]
        public async Task<IActionResult> AssignTask(Guid taskId, [FromBody] AssignUserToTaskDto dto)
        {
            try
            {
                var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
                await _taskService.AssignTaskAsync(taskId, dto, userId);
                return Ok("Task assigned.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{taskId}/unassign")]
        public async Task<IActionResult> UnassignTask(Guid taskId)
        {
            try
            {
                var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
                await _taskService.UnassignTaskAsync(taskId, userId);
                return Ok("Task unassigned.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("statuses")]
        public IActionResult GetStatuses()
        {
            var statuses = Enum.GetNames<TaskItemStatus>();
            return Ok(statuses);
        }
    }
}
