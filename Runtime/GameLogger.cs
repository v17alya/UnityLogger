// -----------------------------------------------------------------------------
// GameLogger.cs
// Namespace : Gamenator.Core.Logging
// Purpose   : Drop‑in replacement for UnityEngine.Debug with per‑class colours,
//             runtime log‑level filter, and zero runtime overhead in builds.
// -----------------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
#if UNITY_EDITOR
using Gamenator.Core.Logging.Utils;
#endif

namespace Gamenator.Core.Logging
{
    /// <summary>
    /// Static logger meant to replace <see cref="UnityEngine.Debug"/>.
    /// Use <c>using Debug = GameLogger;</c> at the top of your files.
    /// </summary>
    public static class GameLogger
    {
        // ---------------------------------------------------------------------
        // Public API (mirrors UnityEngine.Debug where meaningful)
        // ---------------------------------------------------------------------

        /// <summary>Current runtime filter. Anything below this level is ignored (except Fatal).</summary>
        public static LogLevel CurrentLevel { get; private set; } = LogLevel.All;

        /// <summary>Adjusts log level at runtime (e.g. when changing build mode).</summary>
        public static void SetLogLevel(LogLevel level) => CurrentLevel = level;

        /// <summary>Always logs, even in <see cref="LogLevel.None"/>.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [HideInCallstack]
        public static void LogAlways(string msg, UnityEngine.Object ctx = null) =>
            InternalLog(LogLevel.Always, msg, ctx);

        // ---------- Standard Debug signatures (kept 1‑to‑1) ------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [HideInCallstack]
        public static void Log(object message) => InternalLog(LogLevel.Information, message?.ToString());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [HideInCallstack]
        public static void Log(object message, UnityEngine.Object context) =>
            InternalLog(LogLevel.Information, message?.ToString(), context);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [HideInCallstack]
        public static void LogWarning(object message) => InternalLog(LogLevel.Warning, message?.ToString());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [HideInCallstack]
        public static void LogWarning(object message, UnityEngine.Object context) =>
            InternalLog(LogLevel.Warning, message?.ToString(), context);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [HideInCallstack]
        public static void LogError(object message) => InternalLog(LogLevel.Error, message?.ToString());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [HideInCallstack]
        public static void LogError(object message, UnityEngine.Object context) =>
            InternalLog(LogLevel.Error, message?.ToString(), context);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [HideInCallstack]
        public static void LogException(Exception exception, UnityEngine.Object context = null) =>
            UnityEngine.Debug.LogException(exception, context);

        // Lazy‑evaluation overloads (avoid allocations if filtered out)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [HideInCallstack]
        public static void Log(Func<string> supplier) =>
            InternalLog(LogLevel.Information, supplier, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [HideInCallstack]
        public static void LogWarning(Func<string> supplier) =>
            InternalLog(LogLevel.Warning, supplier, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [HideInCallstack]
        public static void LogError(Func<string> supplier) =>
            InternalLog(LogLevel.Error, supplier, null);

        // ---------------------------------------------------------------------
        // Internal implementation
        // ---------------------------------------------------------------------

        // -----------------------------------------------------------------------------
        //  InternalLog
        //  • Keeps Unity’s own stack trace (so file/line remain clickable).
        //  • Relies on [HideInCallstack] to hide every GameLogger frame.
        //  • Adds zero overhead in player builds.
        // -----------------------------------------------------------------------------
        [HideInCallstack]
        private static void InternalLog(LogLevel level, string text, UnityEngine.Object ctx = null)
        {
#if UNITY_EDITOR
            // Abort early if this level is filtered out
            if (!ShouldLog(level)) return;

            // Compose final message (prefix + optional colour for Information)
            string message = Format(text, level);

            // Dispatch to the appropriate Unity call      (Warnings/Errors keep
            // Unity’s default colouring; Information may have per‑class colour)
            switch (level)
            {
                case LogLevel.Warning:
                    UnityEngine.Debug.LogWarning(message, ctx);
                    break;

                case LogLevel.Error:
                case LogLevel.Exception:
                    UnityEngine.Debug.LogError(message, ctx);
                    break;

                default: // Information
                    UnityEngine.Debug.Log(message, ctx);
                    break;
            }

#else
            // ---------------- Player build path (fastest possible) ----------------
            if (!ShouldLog(level)) return;

            switch (level)
            {
                case LogLevel.Warning:
                    UnityEngine.Debug.LogWarning(text, ctx);
                    break;

                case LogLevel.Error:
                case LogLevel.Exception:
                    UnityEngine.Debug.LogError(text, ctx);
                    break;

                default:
                    UnityEngine.Debug.Log(text, ctx);
                    break;
            }
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [HideInCallstack]
        private static void InternalLog(LogLevel level, Func<string> supplier, UnityEngine.Object ctx, int skip = 0)
        {
            if (!ShouldLog(level)) return;
            InternalLog(level, supplier(), ctx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [HideInCallstack]
        private static bool ShouldLog(LogLevel level) =>
            level == LogLevel.Always || (level >= CurrentLevel && CurrentLevel != LogLevel.None);

        // --------------------- Formatting helpers ----------------------------

        [HideInCallstack]
        private static string Format(string raw, LogLevel level)
        {
            Type caller = GetExternalCallerType();
            string prefix = $"[{caller.Name}]"; // class name always, even in prod
#if UNITY_EDITOR
            // colouring only inside the Editor (never included in player builds)
            if (ColorMapping.TryGetColour(caller, out var html))
            {
                // Colour only the first line (prefix) → preview line is coloured, error/warn stay default
                return $"<color=#{html}>{prefix}</color> {raw}";
            }
#endif
            return $"{prefix} {raw}";
        }

        /// <summary>
        /// Walk up the call‑stack until we leave <see cref="GameLogger"/> itself.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [HideInCallstack]
        private static Type GetExternalCallerType()
        {
            var trace = new System.Diagnostics.StackTrace(2, false); // skip InternalLog + Format

            for (int i = 0; i < trace.FrameCount; i++)
            {
                var method = trace.GetFrame(i).GetMethod();
                var type = method?.DeclaringType;

                if (type == null || type == typeof(GameLogger)) continue;   // skip wrapper

                // Walk up through nested, compiler-generated helpers
                while (IsCompilerGenerated(type) && type.DeclaringType != null)
                    type = type.DeclaringType;

                return type ?? typeof(GameLogger);
            }
            return typeof(GameLogger);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [HideInCallstack]
        private static bool IsCompilerGenerated(Type t) =>
            t.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false) ||
            t.Name.StartsWith("<");         // async/iterator/closure helpers
    }
}
