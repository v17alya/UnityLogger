#if UNITY_EDITOR
// -----------------------------------------------------------------------------
// LogColorSettings.cs   (Editor‑only)
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gamenator.Core.Logging.Settings
{
    /// <summary>
    /// ScriptableObject mapping fully‑qualified class names to colours.
    /// Kept in Resources so runtime can load it (Editor‑only code strips colours in players).
    /// </summary>
    [CreateAssetMenu(menuName = "Gamenator/Logging/Log Colour Settings")]
    public class LogColorSettings : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public string TypeName;
            public Color Colour;
        }

        /// <summary>Editable list in the inspector.</summary>
        public List<Entry> Entries = new();

        private Dictionary<string, string> _htmlCache;
        
        // fast HTML cache for Editor runtime
        public bool TryGetHtml(Type t, out string html)
        {
            _htmlCache ??= BuildCache();
            return _htmlCache.TryGetValue(t.FullName, out html);
        }

        private Dictionary<string, string> BuildCache()
        {
            var d = new Dictionary<string, string>(Entries.Count);
            foreach (var e in Entries)
                d[e.TypeName] = ColorUtility.ToHtmlStringRGB(e.Colour);
            return d;
        }
    }
}
#endif
