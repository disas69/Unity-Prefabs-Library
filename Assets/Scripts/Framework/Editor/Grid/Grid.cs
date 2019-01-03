using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Framework.Editor.Grid
{
    public class Grid<T> where T : class, IGridItemContent
    {
        public delegate bool CustomGridFilter(T item);

        private const float Margin = 2f;

        private readonly Dictionary<T, GridItem> _itemsMapping;
        private readonly List<GridItem> _items;
        private readonly int _itemHeight;
        private readonly int _itemWidth;
        private readonly bool _useDefaultFilter;
        private readonly bool _drawDefaultFilter;

        private CustomGridFilter _customGridFilter;
        private GridItem _selectedItem;
        private Rect _scrollRect;
        private int _columnCount;
        private Vector2 _scrollPosition;

        public IGridFilter DefaultFilter = new GridNameFilter();

        public event Action<T> ItemSelected;
        public event Action<T> ItemDoubleClicked;
        public event Action<T> ItemDragged;

        public T SelectedItem
        {
            get
            {
                if (_selectedItem == null)
                {
                    return null;
                }

                return _selectedItem.Content as T;
            }
        }

        public int Count
        {
            get { return _itemsMapping.Count; }
        }

        public int FilteredItemsCount { get; private set; }

        public Grid(int width, int height, bool useDefaultFilter = true, bool drawDefaultFilter = true)
        {
            _itemHeight = height;
            _itemWidth = width;

            _items = new List<GridItem>();
            _itemsMapping = new Dictionary<T, GridItem>();

            _useDefaultFilter = useDefaultFilter;
            _drawDefaultFilter = drawDefaultFilter;
        }

        public void Add(T itemContent)
        {
            var item = new GridItem(_itemWidth, _itemHeight, itemContent);
            item.Clicked += OnItemOnClicked;
            item.DoubleClicked += OnItemDoubleClicked;
            item.Dragged += OnItemDragged;

            _items.Add(item);
            _itemsMapping[itemContent] = item;
        }

        public void SetCustomFilter(CustomGridFilter customGridFilter)
        {
            _customGridFilter = customGridFilter;
        }

        public void Sort()
        {
            _items.Sort((item1, item2) => string.Compare(item1.Name, item2.Name, StringComparison.Ordinal));
        }

        public void Remove(T itemContent)
        {
            GridItem item;
            if (_itemsMapping.TryGetValue(itemContent, out item))
            {
                item.Dispose();

                _items.Remove(item);
                _itemsMapping.Remove(itemContent);
            }
        }

        public void Clear()
        {
            _scrollPosition = Vector2.zero;
            _items.Clear();
            _itemsMapping.Clear();
        }

        public void SelectItem(T item)
        {
            GridItem gridItem;
            if (_itemsMapping.TryGetValue(item, out gridItem))
            {
                OnItemOnClicked(gridItem);
            }
        }

        public void DeselectItem()
        {
            if (_selectedItem != null)
            {
                _selectedItem.IsSelected = false;
            }
        }

        public void ForEach(Action<T> action)
        {
            if (action != null)
            {
                foreach (var virtualizedItem in _itemsMapping)
                {
                    action(virtualizedItem.Key);
                }
            }
        }

        public void Draw()
        {
            _columnCount = Mathf.Max(1, (int) _scrollRect.size.x / _itemWidth);

            using (new EditorGUILayout.VerticalScope())
            {
                if (_drawDefaultFilter)
                {
                    DefaultFilter.Draw();
                }

                DrawContent();
            }
        }

        private void DrawContent()
        {
            using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(_scrollPosition, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUI.skin.box))
            {
                _scrollPosition = scrollViewScope.scrollPosition;

                using (new EditorGUILayout.VerticalScope())
                {
                    FilteredItemsCount = 0;
                    var currentRow = 0;
                    var index = 0;

                    while (index < _items.Count)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            var currentColumn = 0;
                            while (currentColumn < _columnCount && index < _items.Count)
                            {
                                var item = _items[index];
                                if (SatisfiesDefaultFilter(item) && SatisfiesCustomFilter(item))
                                {
                                    FilteredItemsCount++;
                                    item.Draw(IsVisible(currentRow));
                                    currentColumn++;
                                }

                                index++;
                            }
                        }

                        currentRow++;
                    }
                }
            }

            if (Event.current.type == EventType.Repaint)
            {
                _scrollRect = GUILayoutUtility.GetLastRect();
            }
        }

        private bool SatisfiesDefaultFilter(GridItem item)
        {
            if (_useDefaultFilter)
            {
                return DefaultFilter.Match(item.Content);
            }

            return true;
        }

        private bool SatisfiesCustomFilter(GridItem item)
        {
            if (_customGridFilter != null)
            {
                foreach (var virtualizedItem in _itemsMapping)
                {
                    if (virtualizedItem.Value == item)
                    {
                        return _customGridFilter(virtualizedItem.Key);
                    }
                }
            }

            return true;
        }

        private bool IsVisible(int row)
        {
            var height = _itemHeight + Margin;
            if (_scrollPosition.y > (row + 1) * height)
            {
                return false;
            }

            if (row * height > _scrollPosition.y + _scrollRect.height)
            {
                return false;
            }

            return true;
        }

        private void OnItemOnClicked(GridItem item)
        {
            DeselectItem();

            _selectedItem = item;
            item.IsSelected = true;
            EditorWindow.focusedWindow.Repaint();

            if (ItemSelected != null)
            {
                ItemSelected((T) _selectedItem.Content);
            }
        }

        private void OnItemDoubleClicked(GridItem item)
        {
            if (ItemDoubleClicked != null)
            {
                ItemDoubleClicked((T) item.Content);
            }
        }

        private void OnItemDragged(GridItem item)
        {
            if (ItemDragged != null)
            {
                ItemDragged((T) item.Content);
            }
        }

        public void Dispose()
        {
            foreach (var item in _items)
            {
                item.Dispose();
            }

            _items.Clear();
            _itemsMapping.Clear();
        }
    }
}