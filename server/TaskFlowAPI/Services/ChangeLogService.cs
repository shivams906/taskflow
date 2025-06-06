using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskFlowAPI.Data;
using TaskFlowAPI.DTOs;
using TaskFlowAPI.Interfaces;
using TaskFlowAPI.Models;

namespace TaskFlowAPI.Services
{
    public class ChangeLogService : IChangeLogService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ChangeLogService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task LogChange(string entityType, Guid entityId, Guid userId, string summary)
        {
            var change = new ChangeLog
            {
                EntityType = entityType,
                EntityId = entityId,
                ChangedByUserId = userId,
                ChangeSummary = summary,
                Timestamp = DateTime.UtcNow
            };

            _context.ChangeLogs.Add(change);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ChangeLogDto>> GetChangeLogs(string entityType, Guid entityId)
        {
            return await _context.ChangeLogs
                .Where(c => c.EntityType.ToLower() == entityType.ToLower() && c.EntityId == entityId)
                .Include(c => c.ChangedByUser)
                .OrderByDescending(c => c.Timestamp)
                .Select(c => _mapper.Map<ChangeLogDto>(c))
                .ToListAsync();
        }
    }
}
