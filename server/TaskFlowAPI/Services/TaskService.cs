﻿// TaskService.cs
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
        private readonly IChangeLogService _changeLogService;
        private readonly IRoleAccessService _roleAccessService;

        public TaskService(AppDbContext context, IMapper mapper, IChangeLogService changeLogService, IRoleAccessService roleAccessService)
        {
            _context = context;
            _mapper = mapper;
            _changeLogService = changeLogService;
            _roleAccessService = roleAccessService;
        }

        public async Task<PagedResult<TaskDto>> GetTasksForProjectAsync(Guid projectId, Guid userId, QueryParams queryParams)
        {
            var query = _context.Tasks
                .AsNoTracking()
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .Include(t => t.UpdatedBy)
                .Where(t => t.ProjectId == projectId)
                .AsQueryable();

            if (queryParams.Filters != null)
            {
                foreach (var filter in queryParams.Filters)
                {
                    var key = filter.Key.ToLower();
                    var value = filter.Value;

                    if (string.IsNullOrWhiteSpace(value))
                        continue;

                    switch (key)
                    {
                        case "title":
                            query = query.Where(t => t.Title.Contains(value));
                            break;

                        case "assignedtoid":
                            if (Guid.TryParse(value, out var assignedToId))
                                query = query.Where(t => t.AssignedToId == assignedToId);
                            break;

                        case "createdbyid":
                            if (Guid.TryParse(value, out var createdById))
                                query = query.Where(t => t.CreatedById == createdById);
                            break;

                        case "status":
                            if (Enum.TryParse<TaskItemStatus>(value, true, out var statusEnum))
                                query = query.Where(t => t.Status == statusEnum);
                            break;

                            // Add other filters as needed
                    }
                }
            }

            if (!string.IsNullOrEmpty(queryParams.SortBy))
            {
                query = queryParams.SortDesc
                    ? query.OrderByDescending(e => EF.Property<object>(e, queryParams.SortBy))
                    : query.OrderBy(e => EF.Property<object>(e, queryParams.SortBy));
            }

            var pagedTasks = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            var taskDtos = _mapper.Map<List<TaskDto>>(pagedTasks);
            foreach (var taskDto in taskDtos)
            {
                taskDto.Permissions = await _roleAccessService.GetPermissionsForTaskAsync(userId, taskDto.Id);
            }

            int totalItems = await query.CountAsync();

            return new PagedResult<TaskDto>
            {
                Items = taskDtos,
                TotalCount = totalItems,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize,
            };
        }

        public async Task<TaskDto> GetTaskByIdAsync(Guid taskId, Guid userId)
        {
            var task = await _context.Tasks.AsNoTracking().Include(t => t.AssignedTo).Include(t => t.CreatedBy).Include(t => t.UpdatedBy).FirstOrDefaultAsync(t => t.Id == taskId) ?? throw new Exception("Task not found.");
            var taskDto = _mapper.Map<TaskDto>(task);
            taskDto.Permissions = await _roleAccessService.GetPermissionsForTaskAsync(userId, taskId);
            return taskDto;
        }

        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, Guid userId)
        {
            var taskItem = _mapper.Map<TaskItem>(dto);
            taskItem.Id = Guid.NewGuid();
            taskItem.CreatedById = userId;
            taskItem.CreatedAtUtc = DateTime.UtcNow;

            _context.Tasks.Add(taskItem);
            await _context.SaveChangesAsync();

            await _changeLogService.LogChange("Project", dto.ProjectId, userId, $"Task {taskItem.Title} created.");
            await _changeLogService.LogChange("Task", taskItem.Id, userId, $"Task created.");

            var taskDto = _mapper.Map<TaskDto>(taskItem);
            taskDto.Permissions = await _roleAccessService.GetPermissionsForTaskAsync(userId, taskItem.Id);
            return taskDto;
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

            await _changeLogService.LogChange("Task", taskId, userId, $"Status updated from {task.Status} to {statusEnum}");

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
            var task = await _context.Tasks.AsNoTracking().Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == taskId) ?? throw new Exception("Task not found.");

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

            var changes = new List<string>();

            if (task.Title != updated.Title)
            {
                changes.Add($"Title updated from {task.Title} to {updated.Title}");
                task.Title = updated.Title;
            }
            if (task.Description != updated.Description)
            {
                changes.Add($"Description updated from {task.Description} to {updated.Description}");
                task.Description = updated.Description;
            }
            task.Description = updated.Description;
            task.UpdatedById = userId;
            task.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            foreach (var summary in changes)
            {
                await _changeLogService.LogChange("Task", taskId, userId, summary);
            }

            var taskDto = _mapper.Map<TaskDto>(task);
            taskDto.Permissions = await _roleAccessService.GetPermissionsForTaskAsync(userId, task.Id);
            return taskDto;
        }

        public async Task<string> DeleteTaskAsync(Guid taskId, Guid userId)
        {
            var task = await _context.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == taskId) ?? throw new Exception("Task not found.");
            var projectId = task.ProjectId;
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            await _changeLogService.LogChange("Project", projectId, userId, $"Task {task.Title} deleted.");
            await _changeLogService.LogChange("Task", taskId, userId, $"Task deleted");

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

            await _changeLogService.LogChange("Task", taskId, userId, $"Task assigned to {targetUser.Username}");

            return "Task assigned.";
        }

        public async Task<string> UnassignTaskAsync(Guid taskId, Guid userId)
        {
            var task = await _context.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == taskId) ?? throw new Exception("Task not found.");

            task.AssignedToId = null;
            task.UpdatedById = userId;
            task.UpdatedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await _changeLogService.LogChange("Task", taskId, userId, $"Task unassigned");

            return "Task unassigned.";
        }

        public async Task<PagedResult<CommentDto>> GetCommentsForTaskAsync(Guid taskId, Guid userId, QueryParams queryParams)
        {
            var query = _context.Comments
                .AsNoTracking()
                .Include(c => c.CreatedBy)
                .Where(c => c.TaskId == taskId)
                .AsQueryable();

            // Apply filters
            if (queryParams.Filters != null)
            {
                foreach (var filter in queryParams.Filters)
                {
                    var key = filter.Key.ToLower();
                    var value = filter.Value;

                    if (string.IsNullOrWhiteSpace(value))
                        continue;

                    switch (key)
                    {
                        case "createdbyid":
                            if (Guid.TryParse(value, out var createdById))
                                query = query.Where(c => c.CreatedById == createdById);
                            break;
                        case "content":
                            query = query.Where(c => c.Content.Contains(value));
                            break;
                            // Add other filters as needed
                    }
                }
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(queryParams.SortBy))
            {
                query = queryParams.SortDesc
                    ? query.OrderByDescending(e => EF.Property<object>(e, queryParams.SortBy))
                    : query.OrderBy(e => EF.Property<object>(e, queryParams.SortBy));
            }

            // Apply pagination
            var pagedComments = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .Select(c => _mapper.Map<CommentDto>(c))
                .ToListAsync();

            int totalItems = await query.CountAsync();

            return new PagedResult<CommentDto>
            {
                Items = pagedComments,
                TotalCount = totalItems,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize,
            };
        }

        public async Task<CommentDto> CreateCommentAsync(Guid taskId, CreateCommentDto dto, Guid userId)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId) ?? throw new Exception("Task not found.");

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                Content = dto.Content,
                CreatedById = userId,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            await _changeLogService.LogChange("Task", taskId, userId, $"Comment added: {dto.Content}");

            var commentDto = _mapper.Map<CommentDto>(comment);
            return commentDto;
        }
    }
}
