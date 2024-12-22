using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VulpesTool.Editor
{
    [CustomPropertyDrawer(typeof(InterfaceReference<>))]
    public class InterfaceReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var referenceProperty = property.FindPropertyRelative("_reference");

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var type = fieldInfo.FieldType;
            Type interfaceType;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                interfaceType = type.GetGenericArguments()[0].GetGenericArguments()[0];
            }
            else
            {
                interfaceType = type.GetGenericArguments()[0];
            }

            using (var scope = new EditorGUI.PropertyScope(position, label, property))
            {
                var oldObject = referenceProperty.objectReferenceValue;
                EditorGUI.PropertyField(position, referenceProperty, GUIContent.none);

                if (referenceProperty.objectReferenceValue != oldObject)
                {
                    var newObject = referenceProperty.objectReferenceValue;
                    if (newObject != null)
                    {
                        bool isValid = false;

                        if (interfaceType.IsInstanceOfType(newObject))
                        {
                            isValid = true;
                        }
                        else if (newObject is GameObject go)
                        {
                            isValid = go.GetComponent(interfaceType) != null;
                        }
                        else if (newObject is Component comp)
                        {
                            isValid = comp.GetComponent(interfaceType) != null;
                        }

                        if (!isValid)
                        {
                            Debug.LogError($"Object {newObject.name} does not implement interface {interfaceType.Name}");
                            referenceProperty.objectReferenceValue = oldObject;
                        }
                    }
                }
            }

            EditorGUI.EndProperty();
        }
    }
}
