using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace VulpesTool.Editor
{
    public class CustomGUIWindow : EditorWindow
    {
        private UnityEngine.Object target;
        private MethodInfo guiMethod;
        private CreateGUIAttribute attribute;
        private UnityEditor.Editor inspector;

        public void Init(UnityEngine.Object target, MethodInfo method, CreateGUIAttribute attr)
        {
            this.target = target;
            this.guiMethod = method;
            this.attribute = attr;

            if (attribute.ShowInspector)
                inspector = UnityEditor.Editor.CreateEditor(target);

            this.Repaint();
        }

        void OnGUI()
        {
            if (target == null || guiMethod == null)
            {
                EditorGUILayout.HelpBox("Window configuration is invalid", MessageType.Error);
                return;
            }

            try
            {
                if (attribute.ShowInspector)
                {
                    EditorGUILayout.BeginVertical("box");
                    inspector.OnInspectorGUI();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                }

                Color defaultColor = GUI.color;
                GUI.color = attribute.Color.GetColorFromFlags();

                GUITracker.StartTracking(target);
                guiMethod.Invoke(target, null);
                GUITracker.EndTracking();

                GUI.color = defaultColor;
            }
            catch (Exception e)
            {
                EditorGUILayout.HelpBox($"Error: {e.Message}", MessageType.Error);
            }
        }

        void OnDestroy()
        {
            if (inspector != null)
                DestroyImmediate(inspector);
        }
    }
}
