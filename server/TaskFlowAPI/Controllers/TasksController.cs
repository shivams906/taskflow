using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ICurrentSessionProvider _currentSessionProvider;
        private readonly IRoleAccessService _roleAccessService;

        public TasksController(ITaskService taskService, ICurrentSessionProvider currentSessionProvider, IRoleAccessService roleAccessService)
        {
            _taskService = taskService;
            _currentSessionProvider = currentSessionProvider;
            _roleAccessService = roleAccessService;
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetTasksForProject(Guid projectId, [FromQuery] QueryParams queryParams)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            if (!await _roleAccessService.HasPermissionAsync(userId, ProjectPermission.ViewProject, projectId))
                throw new UnauthorizedAccessException("You do not have access");
            var tasks = await _taskService.GetTasksForProjectAsync(projectId, userId, queryParams);
            return Ok(tasks);
        }

        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTaskById(Guid taskId)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            if (!await _roleAccessService.HasPermissionAsync(userId, TaskPermission.ViewTask, taskId))
                throw new UnauthorizedAccessException("You do not have access");
            var task = await _taskService.GetTaskByIdAsync(taskId, userId);
            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto task)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            if (!await _roleAccessService.HasPermissionAsync(userId, ProjectPermission.ManageProject, task.ProjectId))
                throw new UnauthorizedAccessException("You do not have access");
            var createdTask = await _taskService.CreateTaskAsync(task, userId);
            return Ok(createdTask);
        }

        [HttpPut("{taskId}/status")]
        public async Task<IActionResult> UpdateStatus(Guid taskId, [FromBody] UpdateTaskStatusDto dto)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            if (!await _roleAccessService.HasPermissionAsync(userId, TaskPermission.UpdateTaskStatus, taskId))
                throw new UnauthorizedAccessException("You do not have access");
            await _taskService.UpdateTaskStatusAsync(taskId, dto, userId);
            return Ok("Task status updated.");
        }

        [HttpPost("{taskId}/log-time")]
        public async Task<IActionResult> LogTime(Guid taskId, [FromBody] CreateTimeLogDto dto)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            if (!await _roleAccessService.HasPermissionAsync(userId, TaskPermission.LogTime, taskId))
                throw new UnauthorizedAccessException("You do not have access");
            await _taskService.LogTimeAsync(taskId, dto, userId);
            return Ok("Time log saved.");
        }

        [HttpGet("{taskId}/logs")]
        public async Task<IActionResult> GetTimeLogsForTask(Guid taskId, [FromQuery] bool onlyMine = false)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            if (!await _roleAccessService.HasPermissionAsync(userId, TaskPermission.ViewTask, taskId))
                throw new UnauthorizedAccessException("You do not have access");
            var logs = await _taskService.GetTimeLogsAsync(taskId, userId, onlyMine);
            return Ok(logs);
        }

        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] CreateTaskDto updated)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            if (!await _roleAccessService.HasPermissionAsync(userId, TaskPermission.ManageTask, taskId))
                throw new UnauthorizedAccessException("You do not have access");
            var task = await _taskService.UpdateTaskAsync(taskId, updated, userId);
            return Ok(task);
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(Guid taskId)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            if (!await _roleAccessService.HasPermissionAsync(userId, TaskPermission.DeleteTask, taskId))
                throw new UnauthorizedAccessException("You do not have access");
            await _taskService.DeleteTaskAsync(taskId, userId);
            return Ok("Task deleted.");
        }

        [HttpPost("{taskId}/assign")]
        public async Task<IActionResult> AssignTask(Guid taskId, [FromBody] AssignUserToTaskDto dto)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            if (!await _roleAccessService.HasPermissionAsync(userId, TaskPermission.ManageTask, taskId))
                throw new UnauthorizedAccessException("You do not have access");
            await _taskService.AssignTaskAsync(taskId, dto, userId);
            return Ok("Task assigned.");
        }

        [HttpPost("{taskId}/unassign")]
        public async Task<IActionResult> UnassignTask(Guid taskId)
        {
            var userId = _currentSessionProvider.GetUserId() ?? throw new Exception("User ID not found");
            if (!await _roleAccessService.HasPermissionAsync(userId, TaskPermission.ManageTask, taskId))
                throw new UnauthorizedAccessException("You do not have access");
            await _taskService.UnassignTaskAsync(taskId, userId);
            return Ok("Task unassigned.");
        }

        [HttpGet("statuses")]
        public IActionResult GetStatuses()
        {
            var statuses = Enum.GetNames<TaskItemStatus>();
            return Ok(statuses);
        }
    }
}
