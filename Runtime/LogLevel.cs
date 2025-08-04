// File: LogLevel.cs
namespace Gamenator.Core.Logging
{
    /// <summary>Runtime filter for <see cref="GameLogger"/>.</summary>
    public enum LogLevel : int
    {
        /// <summary>All messages are logged.</summary>
        All,
        /// <summary>Information and above.</summary>
        Information,
        /// <summary>Warnings and above.</summary>
        Warning,
        /// <summary>Errors and above.</summary>
        Error,
        /// <summary>Exceptions only.</summary>
        Exception,
        /// <summary>No logging at all.</summary>
        None
    }
}
