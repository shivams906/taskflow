using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlowAPI.Data;
using TaskFlowAPI.Interfaces;

namespace TaskFlowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChangeLogsController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IChangeLogService _changeLogService;

        public ChangeLogsController(AppDbContext appDbContext, IChangeLogService changeLogService)
        {
            _appDbContext = appDbContext;
            _changeLogService = changeLogService;
        }

        [HttpGet("{entityType}/{entityId}")]
        public async Task<IActionResult> GetChangeLog(string entityType, Guid entityId)
        {
            var changes = await _changeLogService.GetChangeLogs(entityType, entityId);

            return Ok(changes);
        }
    }
}
