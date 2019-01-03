using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Framework.Editor.GUIUtilities
{
    public class GUILabelWidth : GUI.Scope
    {
        private readonly float _prevLabelWidth;

        public GUILabelWidth(float labelWidth)
        {
            _prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = labelWidth;
        }

        protected override void CloseScope()
        {
            EditorGUIUtility.labelWidth = _prevLabelWidth;
        }
    }
}