using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CssUI;

/// <summary>
/// Central logging provider for CssUI.
/// Uses Microsoft.Extensions.Logging for modern, flexible logging.
/// </summary>
public static class Log
{
    private static ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;
    private static ILogger _defaultLogger = NullLogger.Instance;

    /// <summary>
    /// Configure CssUI logging with a logger factory.
    /// Call this during application startup to enable logging.
    /// </summary>
    /// <param name="loggerFactory">The logger factory to use for creating loggers.</param>
    public static void Configure(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
        _defaultLogger = _loggerFactory.CreateLogger("CssUI");
    }

    /// <summary>
    /// Get a logger for a specific category/type.
    /// </summary>
    public static ILogger GetLogger<T>() => _loggerFactory.CreateLogger<T>();

    /// <summary>
    /// Get a logger for a specific category name.
    /// </summary>
    public static ILogger GetLogger(string categoryName) => _loggerFactory.CreateLogger(categoryName);

    /// <summary>
    /// Get the default CssUI logger.
    /// </summary>
    public static ILogger Default => _defaultLogger;

    // Convenience methods that mirror common logging patterns

    /// <summary>
    /// Log a debug message.
    /// </summary>
    public static void Debug(string message) => _defaultLogger.LogDebug(message);

    /// <summary>
    /// Log a debug message with format args.
    /// </summary>
    public static void Debug(string message, params object[] args) => _defaultLogger.LogDebug(message, args);

    /// <summary>
    /// Log an informational message.
    /// </summary>
    public static void Info(string message) => _defaultLogger.LogInformation(message);

    /// <summary>
    /// Log an informational message with format args.
    /// </summary>
    public static void Info(string message, params object[] args) => _defaultLogger.LogInformation(message, args);

    /// <summary>
    /// Log a warning message.
    /// </summary>
    public static void Warn(string message) => _defaultLogger.LogWarning(message);

    /// <summary>
    /// Log a warning message with format args.
    /// </summary>
    public static void Warn(string message, params object[] args) => _defaultLogger.LogWarning(message, args);

    /// <summary>
    /// Log an error message.
    /// </summary>
    public static void Error(string message) => _defaultLogger.LogError(message);

    /// <summary>
    /// Log an error message with format args.
    /// </summary>
    public static void Error(string message, params object[] args) => _defaultLogger.LogError(message, args);

    /// <summary>
    /// Log an exception with an error message.
    /// </summary>
    public static void Error(Exception exception, string? message = null) =>
        _defaultLogger.LogError(exception, message ?? exception.Message);

    /// <summary>
    /// Log a critical error message.
    /// </summary>
    public static void Critical(string message) => _defaultLogger.LogCritical(message);

    /// <summary>
    /// Log a critical exception.
    /// </summary>
    public static void Critical(Exception exception, string? message = null) =>
        _defaultLogger.LogCritical(exception, message ?? exception.Message);

    /// <summary>
    /// Log a success/completion message (maps to Information level).
    /// </summary>
    public static void Success(string message) => _defaultLogger.LogInformation("[SUCCESS] {Message}", message);
}
