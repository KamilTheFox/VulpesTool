using UnityEditor;
using UnityEngine;

namespace VulpesTool.Editor
{
    [CustomPropertyDrawer(typeof(EnumSearchWindowAttribute))]
    public class EnumSearchDrawer : PropertyDrawer
    {
        private static SerializedProperty deferredProperty;
        private static Rect deferredButtonRect;
        private static bool shouldOpenWindow;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var attr = attribute as EnumSearchWindowAttribute;

            var fieldRect = new Rect(position.x, position.y, position.width - 25, position.height);
            EditorGUI.PropertyField(fieldRect, property, label);

            var buttonRect = new Rect(fieldRect.xMax + 5, position.y, 20, position.height);
            var icon = string.IsNullOrEmpty(attr.IconName) ?
                EditorGUIUtility.IconContent("d_Search Icon") :
                EditorGUIUtility.IconContent(attr.IconName);

            if (GUI.Button(buttonRect, icon))
            {
                deferredProperty = property.Copy();
                deferredButtonRect = buttonRect;
                shouldOpenWindow = true;
            }

            EditorGUI.EndProperty();

            if (Event.current.type == EventType.Repaint && shouldOpenWindow)
            {
                shouldOpenWindow = false;
                EditorApplication.delayCall += () =>
                {
                    EnumSearchWindow.Show(deferredButtonRect, deferredProperty);
                };
            }
        }
    }
}
