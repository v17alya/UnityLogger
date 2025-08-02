// -----------------------------------------------------------------------------
// ClassPickerWindow.cs
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using OnlySpace.Core.Logging.Settings;

namespace OnlySpace.Core.Logging.Editor
{
    /// <summary>Popup that lists all project classes (via TypeCache) not yet in Settings.</summary>
    internal class ClassPickerWindow : EditorWindow
    {
        private LogColorSettings _settings;
        private SearchField _searchField;
        private string _search;
        private List<Type> _candidates;
        private Vector2 _scroll;

        public static void Open(LogColorSettings settings)
        {
            var win = CreateInstance<ClassPickerWindow>();
            win._settings = settings;
            win._searchField = new SearchField();
            win.RefreshCandidates();
            win.titleContent = new GUIContent("Select Class");
            win.ShowAuxWindow();
        }

        private void RefreshCandidates()
        {
            var existing = new HashSet<string>(_settings.Entries.Select(e => e.TypeName));
            _candidates = TypeCache.GetTypesDerivedFrom<object>()
                                   .Where(t => t.IsClass && !t.IsGenericType && !t.IsNested
                                            && !existing.Contains(t.FullName))
                                   .OrderBy(t => t.FullName)
                                   .ToList();
        }

        private void OnGUI()
        {
            _search = _searchField.OnToolbarGUI(_search);
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            Type picked = null;
            foreach (var t in _candidates.Where(MatchSearch))
                if (GUILayout.Button(t.FullName, EditorStyles.linkLabel))
                    picked = t;

            EditorGUILayout.EndScrollView();

            if (picked != null)
            {
                Undo.RecordObject(_settings, "Add Log Colour Entry");
                _settings.Entries.Add(new LogColorSettings.Entry
                {
                    TypeName = picked.FullName,
                    Colour = Color.white
                });
                EditorUtility.SetDirty(_settings);
                AssetDatabase.SaveAssets();
                Close();
            }
        }

        private bool MatchSearch(Type t) =>
            string.IsNullOrWhiteSpace(_search) ||
            t.FullName.IndexOf(_search, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
