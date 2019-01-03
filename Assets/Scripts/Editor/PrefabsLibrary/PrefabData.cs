using System;
using Assets.Scripts.Framework.Editor.Grid;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor.PrefabsLibrary
{
    [Serializable]
    public class PrefabData : IGridItemContent
    {
        private static GUIStyle _styleNormal;
        private static GUIStyle _styleSelected;

        public GameObject GameObject;
        public string PrefabName;
        public string[] Labels;
        public bool IsSaved;

        private static GUIStyle StyleNormal
        {
            get
            {
                return _styleNormal ?? (_styleNormal = new GUIStyle(GUI.skin.box)
                {
                    normal = {textColor = Color.white}
                });
            }
        }

        private static GUIStyle StyleSelected
        {
            get
            {
                return _styleSelected ?? (_styleSelected = new GUIStyle(GUI.skin.box)
                {
                    normal = {background = Texture2D.whiteTexture, textColor = Color.black}
                });
            }
        }

        public string Name
        {
            get { return PrefabName; }
        }

        public bool IsValid
        {
            get { return IsSaved; }
        }

        public PrefabData(GameObject gameObject)
        {
            GameObject = gameObject;
            PrefabName = gameObject.name;
            Labels = AssetDatabase.GetLabels(gameObject);
        }

        public void Draw(bool isSelected)
        {
            GUILayout.BeginVertical(isSelected ? StyleSelected : StyleNormal);
            {
                GUILayout.Label(new GUIContent(Name), GetTitleStyle(isSelected), GUILayout.Width(150f), GUILayout.Height(20f));
                GUI.DrawTexture(GUILayoutUtility.GetRect(155f, 155f), AssetPreview.GetAssetPreview(GameObject) ?? Texture2D.whiteTexture, ScaleMode.ScaleToFit);
            }
            GUILayout.EndVertical();
        }

        private GUIStyle GetTitleStyle(bool isSelected)
        {
            return new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = {textColor = isSelected ? Color.black : GetTitleColor()}
            };
        }

        private Color GetTitleColor()
        {
            return IsValid ? Color.white : Color.red;
        }
    }
}