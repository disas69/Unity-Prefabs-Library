using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Framework.Editor
{
    public static class Filter
    {
        public static string Draw(string filter, string label, float labelWidth = 100f)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(label, GUILayout.MaxWidth(labelWidth));
                filter = EditorGUILayout.TextField(string.Empty, filter);

                GUI.SetNextControlName("Filter");
                if (GUILayout.Button("\u2573", EditorStyles.toolbarButton, GUILayout.Width(30)))
                {
                    GUI.FocusControl("Filter");
                    filter = string.Empty;
                }
            }
            GUILayout.EndHorizontal();
            return filter;
        }
    }
}