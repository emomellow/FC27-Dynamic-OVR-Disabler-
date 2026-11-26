using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace OVRDisabler.Core.Logging;

public record LogEntry(
    DateTime Timestamp,
    LogLevel Level,
    string Category,
    string Message,
    string? Exception
);

public class InMemoryLogSink
{
    private readonly ConcurrentQueue<LogEntry> _logs = new();
    public event Action<LogEntry>? OnLogAdded;

    public void Add(LogEntry entry)
    {
        _logs.Enqueue(entry);
        while (_logs.Count > 1000)
        {
            _logs.TryDequeue(out _);
        }
        OnLogAdded?.Invoke(entry);
    }

    public IReadOnlyList<LogEntry> GetAllLogs() => _logs.ToArray();
}

public class AppInMemoryLoggerProvider : ILoggerProvider
{
    private readonly InMemoryLogSink _sink;

    public AppInMemoryLoggerProvider(InMemoryLogSink sink)
    {
        _sink = sink;
    }

    public ILogger CreateLogger(string categoryName) => new AppInMemoryLogger(categoryName, _sink);

    public void Dispose() { }
}

public class AppInMemoryLogger : ILogger
{
    private readonly string _categoryName;
    private readonly InMemoryLogSink _sink;

    public AppInMemoryLogger(string categoryName, InMemoryLogSink sink)
    {
        _categoryName = categoryName;
        _sink = sink;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;
        var message = formatter(state, exception);
        _sink.Add(new LogEntry(DateTime.Now, logLevel, _categoryName, message, exception?.ToString()));
    }
}
