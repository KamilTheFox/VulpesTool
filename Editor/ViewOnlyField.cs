using UnityEditor;
using UnityEngine;

namespace VulpesTool.Editor
{
    [CustomPropertyDrawer(typeof(ViewOnlyAttribute))]
    public class ViewOnlyField : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var readOnlyAttr = (ViewOnlyAttribute)attribute;

            bool shouldBeReadOnly = !readOnlyAttr.OnlyInPlayMode || Application.isPlaying;

            if (shouldBeReadOnly)
                GUI.enabled = false;

            EditorGUI.PropertyField(position, property, label, true);

            if (shouldBeReadOnly)
                GUI.enabled = true;
        }
    }
}
