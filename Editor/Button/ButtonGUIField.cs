using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace VulpesTool.Editor
{
    [CustomPropertyDrawer(typeof(ButtonFieldAttribute))]
    public class ButtonGUIField : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var buttonAttr = attribute as ButtonFieldAttribute;
            if (buttonAttr.Position == ButtonPosition.Before || buttonAttr.Position == ButtonPosition.After)
                return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;

            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var buttonAttr = attribute as ButtonFieldAttribute;
            var buttonRect = new Rect();
            var propertyRect = new Rect();

            switch (buttonAttr.Position)
            {
                case ButtonPosition.Before:
                    buttonRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                    propertyRect = new Rect(position.x,
                        position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                        position.width,
                        EditorGUIUtility.singleLineHeight);
                    break;

                case ButtonPosition.After:
                    propertyRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                    buttonRect = new Rect(position.x,
                        position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                        position.width,
                        EditorGUIUtility.singleLineHeight);
                    break;

                case ButtonPosition.Left:
                    float buttonWidth = buttonAttr.ButtonWidth > 0 ? buttonAttr.ButtonWidth : 60f;
                    buttonRect = new Rect(position.x, position.y, buttonWidth, EditorGUIUtility.singleLineHeight);
                    propertyRect = new Rect(position.x + buttonWidth + 5, position.y,
                        position.width - buttonWidth - 5, EditorGUIUtility.singleLineHeight);
                    break;

                case ButtonPosition.Right:
                    buttonWidth = buttonAttr.ButtonWidth > 0 ? buttonAttr.ButtonWidth : 60f;
                    propertyRect = new Rect(position.x, position.y,
                        position.width - buttonWidth - 5, EditorGUIUtility.singleLineHeight);
                    buttonRect = new Rect(position.x + position.width - buttonWidth, position.y,
                        buttonWidth, EditorGUIUtility.singleLineHeight);
                    break;
            }

            EditorGUI.PropertyField(propertyRect, property, label, true);

            Color defaultColor = GUI.color;
            GUI.color = buttonAttr.Color.GetColorFromFlags();

            if (GUI.Button(buttonRect, buttonAttr.ButtonText ?? buttonAttr.MethodName))
            {
                var method = property.serializedObject.targetObject.GetType()
                    .GetMethod(buttonAttr.MethodName,
                        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (method != null)
                {
                    Undo.RecordObject(property.serializedObject.targetObject, $"Execute {property.serializedObject.targetObject}");
                    method.Invoke(property.serializedObject.targetObject, null);
                }
            }
            GUI.color = defaultColor;
        }
    }
}
