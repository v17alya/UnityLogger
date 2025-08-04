#if UNITY_EDITOR
// -----------------------------------------------------------------------------
// ColorMapping.cs   (Editor‑only helper accessed by runtime code)
// -----------------------------------------------------------------------------
using System;
using UnityEngine;
using UnityEditor;
using Gamenator.Core.Logging.Settings;

namespace Gamenator.Core.Logging.Utils
{
    /// <summary>Internal bridge that gives GameLogger fast access to colour data.</summary>
    public static class ColorMapping
    {
        private static LogColorSettings s_settings;
        private static bool             s_searched; 

        /// <summary>Returns <c>true</c> and #RRGGBB when a colour is configured for type.</summary>
        public static bool TryGetColour(Type t, out string html)
        {
            html = default;

            // Perform lazy search exactly once
            if (!s_settings && !s_searched)
            {
                s_searched = true;

                // 1️⃣ Search entire project for *any* LogColorSettings.asset
                string[] guids = AssetDatabase.FindAssets("t:LogColorSettings");
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    s_settings = AssetDatabase.LoadAssetAtPath<LogColorSettings>(path);
                }

                // 2️⃣ Fallback: try Resources root (no hard-coded sub-folders)
                if (!s_settings)
                    s_settings = Resources.Load<LogColorSettings>("LogColorSettings");
            }

            return s_settings && s_settings.TryGetHtml(t, out html);
        }
    }
}
#endif
