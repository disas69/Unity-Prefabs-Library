using UnityEngine;

namespace Assets.Scripts.Framework.Tools
{
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (!_instance)
                {
                    var filePath = GetFilePath();
                    if (string.IsNullOrEmpty(filePath))
                    {
                        _instance = CreateInstance<T>();
                        _instance.hideFlags = HideFlags.HideAndDontSave;
                    }
                    else
                    {
                        _instance = Resources.Load<T>(filePath);
#if UNITY_EDITOR
                        if (_instance == null)
                        {
                            var assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(filePath);
                            if (assets.Length > 0)
                            {
                                _instance = (T) assets[0];
                            }
                        }
#endif
                    }
                }

                return _instance;
            }
        }

        private static string GetFilePath()
        {
            foreach (var customAttribute in typeof(T).GetCustomAttributes(true))
            {
                var resourcePathAttribute = customAttribute as ResourcePathAttribute;
                if (resourcePathAttribute != null)
                {
                    return resourcePathAttribute.FilePath;
                }
            }

            return null;
        }
    }
}