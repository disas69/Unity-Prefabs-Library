using System;
using Assets.Scripts.Framework.Editor;
using Assets.Scripts.Framework.Editor.Grid;
using Assets.Scripts.Framework.Editor.GUIUtilities;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor.PrefabsLibrary
{
    public class PrefabsLibraryWindow : EditorWindow
    {
        private const string Title = "Prefabs Library";

        private LabelGUIWrapper _libraryLabelGui;
        private LabelGUIWrapper _assetLabelGui;
        private IGridFilter _nameFilter;
        private Grid<PrefabData> _grid;
        private PrefabData _selectedPrefabData;
        private Vector2 _contentPosition;
        private bool _showNotSaved;

        [MenuItem("Project/Prefabs Library")]
        public static void OpenWindow()
        {
            var window = GetWindow<PrefabsLibraryWindow>(Title);
            window.minSize = new Vector2(850f, 450f);
            window.Show();
        }

        private void OnEnable()
        {
            _libraryLabelGui = new LabelGUIWrapper();
            _assetLabelGui = new LabelGUIWrapper();

            _grid = new Grid<PrefabData>(150, 175, true, false);
            _grid.ItemSelected += OnItemSelected;
            _grid.ItemDoubleClicked += OnItemDoubleClicked;
            _grid.ItemDragged += OnItemDragged;
            _grid.DefaultFilter = _nameFilter = new GridNameFilter("Name", 40f);

            foreach (var prefabData in PrefabsDataStorage.Instance.PrefabsData)
            {
                _grid.Add(prefabData);
            }

            ResetSelection();
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            DrawEditorLayout();
            if (focusedWindow == this)
            {
                Repaint();
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(PrefabsDataStorage.Instance);
            }
        }

        private void DrawEditorLayout()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(285f), GUILayout.ExpandHeight(true));
                {
                    DrawFilterOptions();
                    GUILayout.FlexibleSpace();
                    DrawFilterResetButton();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    DrawGrid();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawFilterOptions()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                var previousLabels = PrefabsDataStorage.Instance.SelectedLabels;

                using (new GUILabelWidth(40f))
                {
                    using (new GUIEnabled(!_showNotSaved))
                    {
                        _nameFilter.Draw();
                        _libraryLabelGui.Draw(PrefabsDataStorage.Instance);
                        PrefabsDataStorage.Instance.SelectedLabels = _libraryLabelGui.GetLabels(PrefabsDataStorage.Instance);
                    }
                }

                if (previousLabels.Length != PrefabsDataStorage.Instance.SelectedLabels.Length)
                {
                    ResetSelection();
                }

                EditorGUILayout.Space();

                var prevShowNotConfigured = _showNotSaved;
                _showNotSaved = GUILayout.Toggle(_showNotSaved, "Snow Not Saved Prefabs", "Button", GUILayout.Height(32f));

                if (_showNotSaved != prevShowNotConfigured)
                {
                    ResetSelection();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawFilterResetButton()
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            {
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.LabelField(new GUIContent(string.Format("Total items count: {0}", PrefabsDataStorage.Instance.PrefabsData.Count)));
                    EditorGUILayout.LabelField(new GUIContent(string.Format("Items matching filter: {0}", _grid.FilteredItemsCount)));

                    if (GUILayout.Button(new GUIContent("Reset Filter"), GUILayout.Height(32f)))
                    {
                        ResetFilter();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawGrid()
        {
            _contentPosition = EditorGUILayout.BeginScrollView(_contentPosition);
            {
                var dropArea = EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                {
                    _grid.SetCustomFilter(GetCustomGridFilter());
                    _grid.Draw();

                    HandleDragAndDrop(dropArea);
                }
                EditorGUILayout.EndVertical();

                if (_selectedPrefabData != null)
                {
                    DrawSelectedPrefabOptions(_selectedPrefabData);
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void HandleDragAndDrop(Rect dropArea)
        {
            var currentEvent = Event.current;

            switch (currentEvent.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(currentEvent.mousePosition))
                    {
                        return;
                    }

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (currentEvent.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        foreach (var draggedObject in DragAndDrop.objectReferences)
                        {
                            var gameObject = draggedObject as GameObject;
                            if (gameObject != null && PrefabUtility.GetPrefabType(gameObject) == PrefabType.Prefab)
                            {
                                AddPrefabData(gameObject);
                            }
                        }
                    }

                    break;
            }
        }

        private void DrawSelectedPrefabOptions(PrefabData selectedPrefabData)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(52f), GUILayout.ExpandWidth(true));
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.BeginVertical();
                    {
                        if (Selection.activeGameObject != selectedPrefabData.GameObject)
                        {
                            EditorGUILayout.Space();
                            _assetLabelGui.Draw(selectedPrefabData.GameObject);
                            selectedPrefabData.Labels = _assetLabelGui.GetLabels(selectedPrefabData.GameObject);
                        }
                    }
                    EditorGUILayout.EndVertical();

                    GUILayout.FlexibleSpace();

                    EditorGUILayout.BeginHorizontal();
                    {
                        if (!selectedPrefabData.IsSaved)
                        {
                            if (GUILayout.Button(new GUIContent("Save"), GUILayout.Width(64f), GUILayout.Height(35f)))
                            {
                                selectedPrefabData.IsSaved = true;
                                GUI.changed = true;

                                if (_showNotSaved)
                                {
                                    ResetSelection();
                                }
                            }
                        }

                        if (GUILayout.Button(new GUIContent("Remove"), GUILayout.Width(64f), GUILayout.Height(35f)))
                        {
                            if (EditorUtility.DisplayDialog(Title, string.Format("Are you sure you want to remove {0}?", _selectedPrefabData.Name), "Yes", "No"))
                            {
                                _grid.Remove(selectedPrefabData);
                                PrefabsDataStorage.Instance.Remove(selectedPrefabData);
                                GUI.changed = true;
                                ResetSelection();
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        private void ResetFilter()
        {
            _nameFilter.Reset();
            _showNotSaved = false;
            _libraryLabelGui.ClearLabels(PrefabsDataStorage.Instance);

            ResetSelection();
        }

        private void ResetSelection()
        {
            _grid.DeselectItem();
            _selectedPrefabData = null;
        }

        private void OnItemSelected(PrefabData prefabData)
        {
            _selectedPrefabData = prefabData;
        }

        private void OnItemDoubleClicked(PrefabData prefabData)
        {
            Selection.activeGameObject = prefabData.GameObject;
        }

        private void OnItemDragged(PrefabData prefabData)
        {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.objectReferences = new UnityEngine.Object[] { prefabData.GameObject };
            DragAndDrop.StartDrag("Dragging prefab");
        }

        private void AddPrefabData(GameObject gameObject)
        {
            PrefabData prefabData = null;

            _grid.ForEach(data =>
            {
                if (data.GameObject == gameObject)
                {
                    prefabData = data;
                }
            });

            if (prefabData != null)
            {
                if (_selectedPrefabData == null || _selectedPrefabData.GameObject != prefabData.GameObject)
                {
                    EditorUtility.DisplayDialog(Title, string.Format("Prefab Data for {0} already exists", prefabData.GameObject.name), "OK");
                    ResetFilter();
                }
            }
            else
            {
                prefabData = new PrefabData(gameObject);
                _grid.Add(prefabData);
                _grid.Sort();

                PrefabsDataStorage.Instance.Add(prefabData);
                GUI.changed = true;

                _showNotSaved = true;
            }

            _grid.SelectItem(prefabData);
        }

        private Grid<PrefabData>.CustomGridFilter GetCustomGridFilter()
        {
            Grid<PrefabData>.CustomGridFilter filter = null;

            if (_showNotSaved)
            {
                filter = item => !item.IsValid;
            }
            else
            {
                if (PrefabsDataStorage.Instance.SelectedLabels.Length > 0)
                {
                    filter = item =>
                    {
                        foreach (var label in PrefabsDataStorage.Instance.SelectedLabels)
                        {
                            if (Array.IndexOf(item.Labels, label) >= 0)
                            {
                                return true;
                            }
                        }

                        return false;
                    };
                }
            }

            return filter;
        }

        private void OnDisable()
        {
            _grid.ItemSelected -= OnItemSelected;
            _grid.ItemDoubleClicked -= OnItemDoubleClicked;
            _grid.ItemDragged -= OnItemDragged;

            _grid.Dispose();
            _libraryLabelGui.Dispose();
            _assetLabelGui.Dispose();
        }
    }
}