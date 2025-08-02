#if UNITY_EDITOR
// -----------------------------------------------------------------------------
// ColorMapping.cs   (Editor‑only helper accessed by runtime code)
// -----------------------------------------------------------------------------
using System;
using UnityEngine;
using OnlySpace.Core.Logging.Settings;

namespace OnlySpace.Core.Logging.Utils
{
    /// <summary>Internal bridge that gives GameLogger fast access to colour data.</summary>
    public static class ColorMapping
    {
        private static LogColorSettings s_settings;

        /// <summary>Returns <c>true</c> and #RRGGBB when a colour is configured for type.</summary>
        public static bool TryGetColour(Type t, out string html)
        {
            html = default;
            if (!s_settings)
                s_settings = Resources.Load<LogColorSettings>("Settings/GameLogger/LogColorSettings");
            return s_settings && s_settings.TryGetHtml(t, out html);
        }
    }
}
#endif
