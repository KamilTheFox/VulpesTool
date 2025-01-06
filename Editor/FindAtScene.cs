using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace VulpesTool.Editor
{
    [CustomPropertyDrawer(typeof(FindAtSceneAttribute))]
    public class FindAtScene : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                System.Type typeOfObject = fieldInfo.FieldType;
                if (property.objectReferenceValue == null)
                {
                    UnityEngine.Object foundObject = UnityEngine.Object.FindObjectOfType(typeOfObject);
                    if (foundObject != null)
                    {
                        property.objectReferenceValue = foundObject;
                    }
                }
                EditorGUI.PropertyField(position, property, label, true);
                EditorApplication.hierarchyChanged -= OnHierarchyChanged;
                EditorApplication.hierarchyChanged += OnHierarchyChanged;
            }
            else
            {
                EditorGUI.LabelField(position, label, "Use FindOfScene with Object Reference only");
            }

            EditorGUI.EndProperty();
        }
        private void OnHierarchyChanged()
        {
            // Находим все объекты с нашим атрибутом и обновляем их
            var objects = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>();
            foreach (var obj in objects)
            {
                var serializedObject = new SerializedObject(obj);
                var iterator = serializedObject.GetIterator();
                while (iterator.NextVisible(true))
                {
                    if (iterator.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        foreach (var attr in fieldInfo.GetCustomAttributes(typeof(FindAtSceneAttribute), true))
                        {
                            if (iterator.objectReferenceValue == null)
                            {
                                UpdateReference(iterator);
                                serializedObject.ApplyModifiedProperties();
                            }
                        }
                    }
                }
            }
        }
        private void UpdateReference(SerializedProperty property)
        {
            System.Type typeOfObject = fieldInfo.FieldType;
            UnityEngine.Object foundObject = UnityEngine.Object.FindObjectOfType(typeOfObject);
            if (foundObject != null)
            {
                property.objectReferenceValue = foundObject;
            }
        }
    }
    
}
