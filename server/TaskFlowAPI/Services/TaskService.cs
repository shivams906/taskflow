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
            var isAdmin = await _context.ProjectUsers
                .AnyAsync(pu => pu.ProjectId == projectId && pu.UserId == userId);

            if (!isAdmin)
                throw new UnauthorizedAccessException("Only admins can view project tasks.");

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
            var task = await _context.Tasks.Include(t => t.AssignedTo).FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null) throw new Exception("Task not found.");

            var isAdmin = await _context.ProjectUsers.AnyAsync(pu => pu.ProjectId == task.ProjectId && pu.UserId == userId);
            bool isAssigned = task.AssignedToId == userId;

            if (!isAdmin && !isAssigned) throw new UnauthorizedAccessException("Not authorized to view this task.");

            return _mapper.Map<TaskDto>(task);
        }

        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, Guid userId)
        {
            var isAdmin = await _context.ProjectUsers
                .AnyAsync(pu => pu.ProjectId == dto.ProjectId && pu.UserId == userId &&
                    (pu.Role == ProjectRole.Admin || pu.Role == ProjectRole.Owner));

            if (!isAdmin)
                throw new UnauthorizedAccessException("Only project admins can create tasks.");

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
            var task = await _context.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null) throw new Exception("Task not found.");

            bool isAdmin = await _context.ProjectUsers
                .AnyAsync(pu => pu.ProjectId == task.ProjectId && pu.UserId == userId &&
                    (pu.Role == ProjectRole.Admin || pu.Role == ProjectRole.Owner));
            bool isAssignedUser = task.AssignedToId == userId;

            if (!isAdmin && !isAssignedUser)
                throw new UnauthorizedAccessException("Not allowed to update this task.");

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
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null || task.AssignedToId != userId)
                throw new UnauthorizedAccessException("Only the assigned user can log time.");

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
            var task = await _context.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null) throw new Exception("Task not found.");

            bool isAdmin = await _context.ProjectUsers.AnyAsync(pu => pu.ProjectId == task.ProjectId && pu.UserId == userId);
            bool isAssigned = task.AssignedToId == userId;

            if (!isAdmin && !isAssigned) throw new UnauthorizedAccessException("Not authorized to view logs for this task.");

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
            var task = await _context.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null) throw new Exception("Task not found.");

            bool isAdmin = await _context.ProjectUsers
                .AnyAsync(pu => pu.ProjectId == task.ProjectId && pu.UserId == userId &&
                    (pu.Role == ProjectRole.Admin || pu.Role == ProjectRole.Owner));
            if (!isAdmin)
                throw new UnauthorizedAccessException("You don't have permission to update this task.");

            task.Title = updated.Title;
            task.Description = updated.Description;
            task.UpdatedById = userId;
            task.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<TaskDto>(task);
        }

        public async Task<string> DeleteTaskAsync(Guid taskId, Guid userId)
        {
            var task = await _context.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null) throw new Exception("Task not found.");

            bool isAdmin = await _context.ProjectUsers
                .AnyAsync(pu => pu.ProjectId == task.ProjectId && pu.UserId == userId &&
                    (pu.Role == ProjectRole.Admin || pu.Role == ProjectRole.Owner));
            if (!isAdmin)
                throw new UnauthorizedAccessException("Only project admins can delete tasks.");

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return "Task deleted.";
        }

        public async Task<string> AssignTaskAsync(Guid taskId, AssignUserToTaskDto dto, Guid userId)
        {
            var task = await _context.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null) throw new Exception("Task not found.");

            bool isAdmin = await _context.ProjectUsers
                .AnyAsync(pu => pu.ProjectId == task.ProjectId && pu.UserId == userId &&
                    (pu.Role == ProjectRole.Admin || pu.Role == ProjectRole.Owner));
            if (!isAdmin)
                throw new UnauthorizedAccessException("Only project admins can assign tasks.");

            var targetUser = await _context.Users.FindAsync(dto.UserId);
            if (targetUser == null) throw new Exception("User not found.");

            task.AssignedToId = dto.UserId;
            task.UpdatedById = userId;
            task.UpdatedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return "Task assigned.";
        }

        public async Task<string> UnassignTaskAsync(Guid taskId, Guid userId)
        {
            var task = await _context.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null) throw new Exception("Task not found.");

            var isCreator = task.Project.CreatedById == userId;
            var isAdmin = await _context.ProjectUsers
                .AnyAsync(pu => pu.ProjectId == task.ProjectId && pu.UserId == userId &&
                    (pu.Role == ProjectRole.Admin || pu.Role == ProjectRole.Owner));
            if (!isCreator && !isAdmin)
                throw new UnauthorizedAccessException("Only project creator or admins can unassign this task.");

            task.AssignedToId = null;
            task.UpdatedById = userId;
            task.UpdatedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return "Task unassigned.";
        }
    }
}
