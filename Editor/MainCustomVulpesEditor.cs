using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

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
    public class SceneReferenceTracker
    {
        private static bool isProcessing;

        [MenuItem("Tools/vulpesTool/UpdateReferencesScene")]
        private static void UpdateReferences()
        {
            if (isProcessing) return;
            if (EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode) return;

            try
            {
                isProcessing = true;

                for (int i = 0; i < EditorSceneManager.sceneCount; i++)
                {
                    var scene = EditorSceneManager.GetSceneAt(i);
                    if (!scene.isLoaded) continue;

                    var sceneObjects = scene.GetRootGameObjects();
                    var objects = new List<MonoBehaviour>();

                    foreach (var obj in sceneObjects)
                    {
                        objects.AddRange(obj.GetComponentsInChildren<MonoBehaviour>(true));
                    }

                    foreach (var obj in objects)
                    {
                        SerializeMonoBehaviour(obj, scene);
                    }
                }
            }
            finally
            {
                isProcessing = false;
            }
        }
        private static void SerializeMonoBehaviour(MonoBehaviour behaviour, UnityEngine.SceneManagement.Scene scene)
        {
            if (behaviour == null) return;

            if (behaviour.gameObject.scene != scene ||
            string.IsNullOrEmpty(behaviour.gameObject.scene.path) ||
            !behaviour.gameObject.scene.isLoaded)
            {
                return;
            }

            var serializedObject = new SerializedObject(behaviour);
            var properties = new List<SerializedProperty>();

            var iterator = serializedObject.GetIterator();
            while (iterator.NextVisible(true))
            {
                properties.Add(serializedObject.FindProperty(iterator.propertyPath));
            }

            bool modified = false;

            foreach (var property in properties)
            {
                var fieldInfo = behaviour.GetType().GetField(property.name,
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic);

                if (fieldInfo == null) continue;

                var attributes = fieldInfo.GetCustomAttributes(typeof(FindAtSceneAttribute), true);

                if (attributes.Length > 0 &&
                    property.propertyType == SerializedPropertyType.ObjectReference &&
                    property.objectReferenceValue == null)
                {
                    System.Type typeOfObject = fieldInfo.FieldType;

                    UnityEngine.Object foundObject = null;

                    if (scene.isLoaded)
                    {
                        var sceneObjects = scene.GetRootGameObjects();
                        foreach (var sceneObj in sceneObjects)
                        {
                            var component = sceneObj.GetComponentInChildren(typeOfObject, true);
                            if (component != null)
                            {
                                foundObject = component;
                                break;
                            }
                        }
                    }

                    if (foundObject != null)
                    {
                        property.objectReferenceValue = foundObject;
                        modified = true;
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
