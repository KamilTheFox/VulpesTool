using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace VulpesTool.Editor
{
    public class MainCustomVulpesEditor : UnityEditor.Editor
    {
        List<IDrawerGUI> drawerGUIs = new List<IDrawerGUI>()
        {
            new CustomGUIMethodEditor(),
            new ButtonDrawerGUI(),
        };

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            drawerGUIs.ForEach(x =>
            {
                x.OnEnable(this);
                x.OnInspectorGUI();
                x.OnDisable();
            });
        }

    }
    [InitializeOnLoad]
    public class SceneReferenceTracker
    {
        private static bool initialized = false;

        static SceneReferenceTracker()
        {
            if (!initialized)
            {
                EditorApplication.hierarchyChanged += OnHierarchyChanged;

                initialized = true;
            }
        }
        private static void OnHierarchyChanged()
        {

            var objects = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>();

            foreach (var obj in objects)
            {
                var serializedObject = new SerializedObject(obj);
                var properties = new List<SerializedProperty>();

                var iterator = serializedObject.GetIterator();

                while (iterator.NextVisible(true))
                {
                    properties.Add(serializedObject.FindProperty(iterator.propertyPath));
                }

                bool modified = false;

                foreach (var property in properties)
                {
                    var fieldInfo = obj.GetType().GetField(property.name,
                        System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.NonPublic);

                    if (fieldInfo != null)
                    {
                        var attributes = fieldInfo.GetCustomAttributes(typeof(FindAtSceneAttribute), true);

                        if (attributes.Length > 0 &&
                            property.propertyType == SerializedPropertyType.ObjectReference &&
                            property.objectReferenceValue == null)
                        {
                            System.Type typeOfObject = fieldInfo.FieldType;
                            UnityEngine.Object foundObject = UnityEngine.Object.FindObjectOfType(typeOfObject);

                            if (foundObject != null)
                            {
                                property.objectReferenceValue = foundObject;
                                modified = true;
                            }
                        }
                    }
                }
                if (modified)
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}
