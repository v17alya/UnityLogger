// -----------------------------------------------------------------------------
// LogColorEditorWindow.cs
// -----------------------------------------------------------------------------
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Gamenator.Core.Logging.Settings;

namespace Gamenator.Core.Logging.Editor
{
    /// <summary>Tool window: add/remove class→colour mappings with live search.</summary>
    public class LogColorEditorWindow : EditorWindow
    {
        private LogColorSettings _settings;
        private SearchField _searchField;
        private string _search;
        private Vector2 _scroll;

        [MenuItem("Tools/Logging/Log Colour Configurator")]
        private static void Open() => GetWindow<LogColorEditorWindow>("Log Colours");

        private void OnEnable()
        {
            _settings = LoadOrCreateSettings();
            _searchField = new SearchField();
        }

        private void OnGUI()
        {
            DrawToolbar();
            DrawList();
        }

        // ----------------------------- UI helpers ----------------------------
        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            _search = _searchField.OnToolbarGUI(_search);

            if (GUILayout.Button("Add Class…", EditorStyles.toolbarButton, GUILayout.Width(90)))
                ClassPickerWindow.Open(_settings);

            EditorGUILayout.EndHorizontal();
        }

        private void DrawList()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            var view = string.IsNullOrWhiteSpace(_search)
                       ? _settings.Entries
                       : _settings.Entries.Where(e => e.TypeName.IndexOf(_search, StringComparison.OrdinalIgnoreCase) >= 0);

            for (int i = 0; i < _settings.Entries.Count; i++)
            {
                var entry = _settings.Entries[i];
                if (!view.Contains(entry)) continue;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(entry.TypeName, GUILayout.MinWidth(300));

                Color newCol = EditorGUILayout.ColorField(entry.Colour);
                if (newCol != entry.Colour)
                {
                    Undo.RecordObject(_settings, "Change Log Colour");
                    entry.Colour = newCol;
                    _settings.Entries[i] = entry;
                    EditorUtility.SetDirty(_settings);
                }

                if (GUILayout.Button("–", GUILayout.Width(18)))
                {
                    Undo.RecordObject(_settings, "Remove Log Colour");
                    _settings.Entries.RemoveAt(i);
                    EditorUtility.SetDirty(_settings);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        // --------------------- Settings asset helpers ------------------------
        private static LogColorSettings LoadOrCreateSettings()
        {
            // Update this path if you prefer another default location
            const string DefaultAssetPath = "Assets/Editor/Resources/GameLogger/Settings/LogColorSettings.asset";

            // ── 1️⃣ Search entire project for an existing asset ───────────────────
            string[] guids = AssetDatabase.FindAssets("t:LogColorSettings");
            if (guids.Length > 0)
            {
                string foundPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                var existing = AssetDatabase.LoadAssetAtPath<LogColorSettings>(foundPath);
                if (existing)
                {
                    Debug.Log($"<color=green>[LogColours]</color> Found existing settings at {foundPath}");
                    return existing;
                }
            }

            // ── 2️⃣ If not found, create at the default path ──────────────────────
            // Ensure folder chain exists (recursive)
            string dir = System.IO.Path.GetDirectoryName(DefaultAssetPath);
            CreateFoldersIfNeeded(dir);

            var settings = ScriptableObject.CreateInstance<LogColorSettings>();
            AssetDatabase.CreateAsset(settings, DefaultAssetPath);
            AssetDatabase.SaveAssets();

            Debug.Log($"<color=yellow>[LogColours]</color> Created new settings asset at {DefaultAssetPath}");
            return settings;
        }


        /// <summary>
        /// Recursively creates any missing folder in a Unity‑safe way
        /// (ensures .meta files are generated).
        /// </summary>
        private static void CreateFoldersIfNeeded(string fullPath)
        {
            // fullPath => "Assets/Core/Editor/Resources/Settings/GameLogger"
            var segments = fullPath.Split('/');
            string current = segments[0];                     // "Assets"
            for (int i = 1; i < segments.Length; i++)
            {
                string next = $"{current}/{segments[i]}";
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, segments[i]);
                current = next;
            }
        }
    }
}
