using System;
using System.Globalization;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Framework.Editor
{
    public class LabelGUIWrapper : IDisposable
    {
        private const string TypeName = "UnityEditor.LabelGUI, UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";

        private readonly object _labelGUI;

        public LabelGUIWrapper()
        {
            var type = Type.GetType(TypeName);
            if (type != null)
            {
                _labelGUI = Activator.CreateInstance(type, true);
                CallMethod("OnEnable");
            }
            else
            {
                Debug.LogError("Failed to create an instance of UnityEditor.LabelGUI!");
            }
        }

        public string[] GetLabels(UnityEngine.Object asset)
        {
            return AssetDatabase.GetLabels(asset);
        }

        public void Draw(UnityEngine.Object asset)
        {
            CallMethod("OnLabelGUI", asset);
        }

        public void ClearLabels(UnityEngine.Object asset)
        {
            AssetDatabase.ClearLabels(asset);
            CallMethod("InvalidateLabels");
        }

        public void Dispose()
        {
            CallMethod("OnDisable");
        }

        private void CallMethod(string methodName, UnityEngine.Object asset = null)
        {
            if (_labelGUI != null)
            {
                var methodInfo = _labelGUI.GetType().GetMethod(methodName);
                if (methodInfo != null)
                {
                    var param = asset != null ? new object[] { new[] { asset } } : new object[] { };
                    methodInfo.Invoke(_labelGUI, BindingFlags.Instance, null, param, CultureInfo.InvariantCulture);
                }
            }
        }
    }
}