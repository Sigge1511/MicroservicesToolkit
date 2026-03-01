using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoggingService.Data;
using LoggingService.Models;

namespace LoggingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    private readonly LoggingDbContext _context;

    public LogsController(LoggingDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<LogEntry>> CreateLog([FromBody] CreateLogRequest request)
    {
        var logEntry = new LogEntry
        {
            ServiceName = request.ServiceName,
            Level = request.Level,
            Message = request.Message,
            Time = request.Time
        };

        _context.LogEntries.Add(logEntry);
        await _context.SaveChangesAsync();

        Console.WriteLine($"[{logEntry.Time:yyyy-MM-dd HH:mm:ss}] [{logEntry.Level}] {logEntry.ServiceName}: {logEntry.Message}");

        return CreatedAtAction(nameof(GetLogs), new { id = logEntry.Id }, logEntry);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LogEntry>>> GetLogs(
        [FromQuery] string? serviceName = null,
        [FromQuery] string? level = null,
        [FromQuery] int limit = 100)
    {
        var query = _context.LogEntries.AsQueryable();

        if (!string.IsNullOrEmpty(serviceName))
        {
            query = query.Where(l => l.ServiceName == serviceName);
        }

        if (!string.IsNullOrEmpty(level))
        {
            query = query.Where(l => l.Level == level);
        }

        var logs = await query
            .OrderByDescending(l => l.Time)
            .Take(limit)
            .ToListAsync();

        return Ok(logs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LogEntry>> GetLogById(int id)
    {
        var log = await _context.LogEntries.FindAsync(id);

        if (log == null)
        {
            return NotFound();
        }

        return Ok(log);
    }
}

public class CreateLogRequest
{
    public string ServiceName { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Time { get; set; }
}
