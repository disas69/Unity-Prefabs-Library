using UnityEngine;

namespace Assets.Scripts.Framework.Editor.GUIUtilities
{
    public class GUIEnabled : GUI.Scope
    {
        private readonly bool _pevEnabled;

        public GUIEnabled(bool enabled)
        {
            _pevEnabled = GUI.enabled;
            GUI.enabled = enabled;
        }

        protected override void CloseScope()
        {
            GUI.enabled = _pevEnabled;
        }
    }
}