// TaskService.cs
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskFlowAPI.Data;
using TaskFlowAPI.DTOs;
using TaskFlowAPI.Interfaces;
using TaskFlowAPI.Models;
using TaskFlowAPI.Models.Enums;

namespace TaskFlowAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TaskService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<TaskDto>> GetTasksForProjectAsync(Guid projectId, Guid userId)
        {
            var tasks = await _context.Tasks
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .Where(t => t.ProjectId == projectId)
                .Select(t => _mapper.Map<TaskDto>(t))
                .ToListAsync();

            return tasks;
        }

        public async Task<TaskDto> GetTaskByIdAsync(Guid taskId, Guid userId)
        {
            var task = await _context.Tasks.Include(t => t.AssignedTo).FirstOrDefaultAsync(t => t.Id == taskId) ?? throw new Exception("Task not found.");
            return _mapper.Map<TaskDto>(task);
        }

        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, Guid userId)
        {
            var taskItem = _mapper.Map<TaskItem>(dto);
            taskItem.Id = Guid.NewGuid();
            taskItem.CreatedById = userId;
            taskItem.CreatedAtUtc = DateTime.UtcNow;

            _context.Tasks.Add(taskItem);
            await _context.SaveChangesAsync();

            return _mapper.Map<TaskDto>(taskItem);
        }

        public async Task<string> UpdateTaskStatusAsync(Guid taskId, UpdateTaskStatusDto dto, Guid userId)
        {
            var task = await _context.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == taskId) ?? throw new Exception("Task not found.");

            if (!Enum.TryParse<TaskItemStatus>(dto.NewStatus, true, out var statusEnum))
                throw new ArgumentException("Invalid task status.");

            task.Status = statusEnum;
            task.UpdatedById = userId;
            task.UpdatedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return "Task status updated.";
        }

        public async Task<string> LogTimeAsync(Guid taskId, CreateTimeLogDto dto, Guid userId)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId) ?? throw new Exception("Task not found.");

            var log = _mapper.Map<TaskTimeLog>(dto);
            log.Id = Guid.NewGuid();
            log.TaskItemId = taskId;
            log.UserId = userId;
            log.CreatedById = userId;
            log.CreatedAtUtc = DateTime.UtcNow;

            _context.TaskTimeLogs.Add(log);
            await _context.SaveChangesAsync();

            return "Time log saved.";
        }

        public async Task<List<TimeLogDto>> GetTimeLogsAsync(Guid taskId, Guid userId, bool onlyMine)
        {
            var task = await _context.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == taskId) ?? throw new Exception("Task not found.");

            var logsQuery = _context.TaskTimeLogs.Where(t => t.TaskItemId == taskId);
            if (onlyMine) logsQuery = logsQuery.Where(t => t.UserId == userId);

            var logs = await logsQuery.Include(l => l.User)
                .OrderByDescending(l => l.StartTime)
                .Select(l => _mapper.Map<TimeLogDto>(l))
                .ToListAsync();

            return logs;
        }

        public async Task<TaskDto> UpdateTaskAsync(Guid taskId, CreateTaskDto updated, Guid userId)
        {
            var task = await _context.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == taskId) ?? throw new Exception("Task not found.");

            task.Title = updated.Title;
            task.Description = updated.Description;
            task.UpdatedById = userId;
            task.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<TaskDto>(task);
        }

        public async Task<string> DeleteTaskAsync(Guid taskId, Guid userId)
        {
            var task = await _context.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == taskId) ?? throw new Exception("Task not found.");

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return "Task deleted.";
        }

        public async Task<string> AssignTaskAsync(Guid taskId, AssignUserToTaskDto dto, Guid userId)
        {
            var task = await _context.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == taskId) ?? throw new Exception("Task not found.");

            var targetUser = await _context.Users.FindAsync(dto.UserId) ?? throw new Exception("User not found.");
            task.AssignedToId = dto.UserId;
            task.UpdatedById = userId;
            task.UpdatedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return "Task assigned.";
        }

        public async Task<string> UnassignTaskAsync(Guid taskId, Guid userId)
        {
            var task = await _context.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == taskId) ?? throw new Exception("Task not found.");

            task.AssignedToId = null;
            task.UpdatedById = userId;
            task.UpdatedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return "Task unassigned.";
        }
    }
}
