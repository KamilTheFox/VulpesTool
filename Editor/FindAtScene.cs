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
            }
            else
            {
                EditorGUI.LabelField(position, label, "Use FindOfScene with Object Reference only");
            }

            EditorGUI.EndProperty();
        }
        
    }
    
}
