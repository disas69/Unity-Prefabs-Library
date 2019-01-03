using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Framework.Editor.Grid
{
    public class GridItem
    {
        private readonly int _width;
        private readonly int _height;

        public string Name
        {
            get { return Content.Name; }
        }

        public bool IsSelected { get; set; }
        public IGridItemContent Content { get; private set; }

        public event Action<GridItem> Clicked;
        public event Action<GridItem> DoubleClicked;
        public event Action<GridItem> Dragged;

        public GridItem(int width, int height, IGridItemContent content)
        {
            _width = width;
            _height = height;
            Content = content;
        }

        public void Draw(bool visible)
        {
            var style = new GUIStyle(GUI.skin.box) {margin = new RectOffset(2, 2, 2, 2)};

            EditorGUILayout.BeginVertical(style, GUILayout.MinWidth(_width), GUILayout.MinHeight(_height), GUILayout.MaxWidth(_width), GUILayout.MaxHeight(_height));
            {
                if (visible)
                {
                    Content.Draw(IsSelected);
                }
                else
                {
                    GUILayoutUtility.GetRect(0.0f, 0.0f);
                }
            }
            EditorGUILayout.EndVertical();

            var currentEvent = Event.current;
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                if (currentEvent.type == EventType.MouseDown)
                {
                    if (Clicked != null)
                    {
                        Clicked(this);
                    }

                    if (Event.current.clickCount == 2)
                    {
                        if (DoubleClicked != null)
                        {
                            DoubleClicked(this);
                        }
                    }

                    Event.current.Use();
                }
                else if (Event.current.type == EventType.MouseDrag && IsSelected)
                {
                    if (Dragged != null)
                    {
                        Dragged(this);
                    }

                    Event.current.Use();
                }
            }
        }

        public void Dispose()
        {
            Clicked = null;
            DoubleClicked = null;
            Dragged = null;
        }
    }
}