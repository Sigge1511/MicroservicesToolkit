using Xunit;
using Microsoft.EntityFrameworkCore;
using LoggingService.Controllers;
using LoggingService.Data;
using LoggingService.Models;
using Microsoft.AspNetCore.Mvc;

namespace LoggingService.Tests;

public class LogsControllerTests
{
    private LoggingDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<LoggingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new LoggingDbContext(options);
    }

    [Fact]
    public async Task CreateLog_AddsLogToDatabase()
    {
        using var context = CreateInMemoryContext();
        var controller = new LogsController(context);
        var request = new CreateLogRequest
        {
            ServiceName = "TestService",
            Level = "INFO",
            Message = "Test message",
            Time = DateTime.UtcNow
        };

        var result = await controller.CreateLog(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var log = Assert.IsType<LogEntry>(createdResult.Value);
        Assert.Equal("TestService", log.ServiceName);
        Assert.Equal("INFO", log.Level);
        Assert.Equal("Test message", log.Message);
    }

    [Fact]
    public async Task GetLogs_ReturnsAllLogs()
    {
        using var context = CreateInMemoryContext();
        
        context.LogEntries.AddRange(
            new LogEntry { ServiceName = "Service1", Level = "INFO", Message = "Message 1", Time = DateTime.UtcNow },
            new LogEntry { ServiceName = "Service2", Level = "ERROR", Message = "Message 2", Time = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        var controller = new LogsController(context);
        var result = await controller.GetLogs();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var logs = Assert.IsAssignableFrom<IEnumerable<LogEntry>>(okResult.Value);
        Assert.Equal(2, logs.Count());
    }

    [Fact]
    public async Task GetLogs_FiltersBy ServiceName()
    {
        using var context = CreateInMemoryContext();
        
        context.LogEntries.AddRange(
            new LogEntry { ServiceName = "Service1", Level = "INFO", Message = "Message 1", Time = DateTime.UtcNow },
            new LogEntry { ServiceName = "Service2", Level = "INFO", Message = "Message 2", Time = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        var controller = new LogsController(context);
        var result = await controller.GetLogs(serviceName: "Service1");

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var logs = Assert.IsAssignableFrom<IEnumerable<LogEntry>>(okResult.Value).ToList();
        Assert.Single(logs);
        Assert.Equal("Service1", logs[0].ServiceName);
    }

    [Fact]
    public async Task GetLogs_FiltersByLevel()
    {
        using var context = CreateInMemoryContext();
        
        context.LogEntries.AddRange(
            new LogEntry { ServiceName = "Service1", Level = "INFO", Message = "Message 1", Time = DateTime.UtcNow },
            new LogEntry { ServiceName = "Service1", Level = "ERROR", Message = "Message 2", Time = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        var controller = new LogsController(context);
        var result = await controller.GetLogs(level: "ERROR");

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var logs = Assert.IsAssignableFrom<IEnumerable<LogEntry>>(okResult.Value).ToList();
        Assert.Single(logs);
        Assert.Equal("ERROR", logs[0].Level);
    }

    [Fact]
    public async Task GetLogById_ReturnsLog_WhenExists()
    {
        using var context = CreateInMemoryContext();
        
        var log = new LogEntry
        {
            ServiceName = "TestService",
            Level = "INFO",
            Message = "Test",
            Time = DateTime.UtcNow
        };
        context.LogEntries.Add(log);
        await context.SaveChangesAsync();

        var controller = new LogsController(context);
        var result = await controller.GetLogById(log.Id);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedLog = Assert.IsType<LogEntry>(okResult.Value);
        Assert.Equal(log.Id, returnedLog.Id);
    }

    [Fact]
    public async Task GetLogById_ReturnsNotFound_WhenDoesNotExist()
    {
        using var context = CreateInMemoryContext();
        var controller = new LogsController(context);

        var result = await controller.GetLogById(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetLogs_RespectsLimit()
    {
        using var context = CreateInMemoryContext();
        
        for (int i = 0; i < 10; i++)
        {
            context.LogEntries.Add(new LogEntry
            {
                ServiceName = "Service",
                Level = "INFO",
                Message = $"Message {i}",
                Time = DateTime.UtcNow
            });
        }
        await context.SaveChangesAsync();

        var controller = new LogsController(context);
        var result = await controller.GetLogs(limit: 5);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var logs = Assert.IsAssignableFrom<IEnumerable<LogEntry>>(okResult.Value);
        Assert.Equal(5, logs.Count());
    }
}
