using System;
using UnityEngine;

namespace Assets.Scripts.Framework.Tools
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ResourcePathAttribute : Attribute
    {
        public string FilePath { get; private set; }

        public ResourcePathAttribute(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("Invalid Resource Path!");
            }

            FilePath = path;
        }
    }
}