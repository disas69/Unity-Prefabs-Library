using System;
using System.Collections.Generic;
using Assets.Scripts.Framework.Tools;
using UnityEngine;

namespace Assets.Scripts.Editor.PrefabsLibrary
{
    [ResourcePath(AssetPath)]
    [CreateAssetMenu(fileName = "PrefabsDataStorage")]
    public class PrefabsDataStorage : ScriptableSingleton<PrefabsDataStorage>
    {
        private const string AssetPath = "Assets/Scripts/Editor/PrefabsDataStorage.asset";

        public string[] SelectedLabels = { };
        public List<PrefabData> PrefabsData = new List<PrefabData>();

        public void Add(PrefabData data)
        {
            PrefabsData.Add(data);
            PrefabsData.Sort((item1, item2) => string.Compare(item1.Name, item2.Name, StringComparison.Ordinal));
        }

        public void Remove(PrefabData data)
        {
            PrefabsData.Remove(data);
        }
    }
}